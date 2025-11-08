using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.Enum;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Kria.Core.Pleno.Lib.BLL
{
    public class PedagioBLL(
        IPedagioDAO pedagioDAO,
        IConfigurationDao configurationDao,
        PedagioValidator pedagioValidator,
        RegistroPedagioValidator registroPedagioValidator
    ) : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO = pedagioDAO;
        private readonly IConfigurationDao _configurationDao = configurationDao;
        private readonly PedagioValidator _pedagioValidator = pedagioValidator;
        private readonly RegistroPedagioValidator _registroPedagioValidator = registroPedagioValidator;

        public async Task ProcessarLotePedagioAsync()
        {
            int pacotesProcessamento = TryGetInt("Configuracoes:PacotesProcessamento", 1000);
            int dadosEmMemoria = TryGetInt("Configuracoes:DadosEmMemoria", 10000);
            int threads = TryGetInt("Configuracoes:Threads", Math.Max(1, Environment.ProcessorCount / 2));
            string candidato = _configurationDao.PegarChave("Candidado") ?? string.Empty;

            DateTime? ultimaData = null;

            int totalLoteGrande = 0;
            int totalSubLote = 0;
            int totalItemSubLote = 0;
            int totalErro = 0;

            while (true)
            {
                var loteGrande = _pedagioDAO.ObterLote(ultimaData, dadosEmMemoria).ToList();
                if (loteGrande.Count == 0) break;

                totalLoteGrande = loteGrande.Count();
                foreach (var subLote in loteGrande.Chunk(pacotesProcessamento))
                {
                    var primeiro = subLote[0];
                    var ultimo = subLote[^1];

                    var dataReferencia = primeiro.DtCriacao.ToString("dd/MM/yyyy");
                    var numeroArquivo = GerarId.ObterProximoNumeroArquivo(dataReferencia);

                    var pedagio = new Pedagio
                    {
                        Candidato = candidato,
                        DataReferencia = dataReferencia,
                        NumeroArquivo = numeroArquivo,
                        Registros = new()
                    };

                    totalSubLote++;

                    var errosDoLote = await ProcessarSubLoteAsync(
                        _registroPedagioValidator,
                        threads,
                        numeroArquivo,
                        subLote,
                        pedagio
                    ).ConfigureAwait(false);

                    totalErro += errosDoLote;

                    var resultPedagio = await _pedagioValidator.ValidateAsync(pedagio).ConfigureAwait(false);
                    if (!resultPedagio.IsValid)
                    {
                        var erros = string.Join(", ", resultPedagio.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                        ultimaData = ultimo.DtCriacao;
                        continue;
                    }

                    // await _pedagioDAO.SalvarPedagioAsync(pedagio).ConfigureAwait(false);

                    totalItemSubLote += pedagio.Registros.Count;
                    Terminal.Mensagem(
                        $"Lote {numeroArquivo} processado com {pedagio.Registros.Count} registros (Data {dataReferencia})",
                        "Processado: ",
                        ConsoleColor.Green
                    );

                    ultimaData = ultimo.DtCriacao;
                }
            }

            Terminal.Mensagem(
                $"Processamento concluído — Lotes: {totalSubLote} | Registros processados: {totalItemSubLote} / {totalLoteGrande} | Error: {totalErro}.",
                "Finalizado: ",
                ConsoleColor.Green
            );
        }

        private static async Task<int> ProcessarSubLoteAsync(
            RegistroPedagioValidator registroPedagioValidator,
            int threads,
            int numeroArquivo,
            TabTransacoes[] subLote,
            Pedagio pedagio)
        {
            var buffer = new ConcurrentQueue<RegistroPedagio>();
            int erros = 0;

            await Parallel.ForEachAsync(
                subLote,
                new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, threads) },
                async (item, _) =>
                {
                    var registro = new RegistroPedagio
                    {
                        GUID = Guid.NewGuid().ToString(),
                        CodigoPracaPedagio = int.TryParse(item.CodigoPracaPedagio, out var codigo) ? codigo : 0,
                        CodigoCabine = item.CodigoCabine,
                        Instante = item.Instante,
                        Sentido = item.Sentido,
                        TipoVeiculo = (int)ETipoVeiculo.Comercial,
                        Isento = item.Isento,
                        Evasao = item.Evasao,
                        TipoCobrancaEfetuada = item.TipoCobranca,
                        ValorDevido = item.ValorDevido,
                        ValorArrecadado = item.ValorArrecadado,
                        MultiplicadorTarifa = 0
                    };

                    var result = await registroPedagioValidator.ValidateAsync(registro).ConfigureAwait(false);
                    if (!result.IsValid)
                    {
                        Interlocked.Increment(ref erros);
                        var errosStr = string.Join(", ", result.Errors.Select(e => $"Arquivo: {numeroArquivo} {e.PropertyName}: {e.ErrorMessage}: Valor:{e.AttemptedValue}"));
                        return;
                    }

                    buffer.Enqueue(registro);
                }
            ).ConfigureAwait(false);

            if (!buffer.IsEmpty)
                pedagio.Registros.AddRange(buffer);

            return erros;
        }

        private int TryGetInt(string key, int @default)
            => int.TryParse(_configurationDao.PegarChave(key), out var val) ? val : @default;
    }
}
