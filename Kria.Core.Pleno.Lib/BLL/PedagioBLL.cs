using FluentValidation.Results;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.Enum;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kria.Core.Pleno.Lib.BLL
{
    public class PedagioBLL : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO;
        private readonly IConfigurationDAO _configurationDAO;
        private readonly IErroCollectorDAO _erroCollectorDAO;
        private readonly IPublicarDesafioDAO _publicarDesafioDAO;
        private readonly PedagioValidator _pedagioValidator;
        private readonly RegistroPedagioValidator _registroPedagioValidator;

        private int pacotesProcessamento, dadosEmMemoria, threads, logError;
        private string candidato;

        public PedagioBLL(
            IPedagioDAO pedagioDAO,
            IConfigurationDAO configurationDAO,
            IErroCollectorDAO erroCollectorDAO,
            IPublicarDesafioDAO publicarDesafioDAO,
            PedagioValidator pedagioValidator,
            RegistroPedagioValidator registroPedagioValidator)
        {
            _pedagioDAO = pedagioDAO;
            _configurationDAO = configurationDAO;
            _pedagioValidator = pedagioValidator;
            _registroPedagioValidator = registroPedagioValidator;
            _erroCollectorDAO = erroCollectorDAO;
            _publicarDesafioDAO = publicarDesafioDAO;

            pacotesProcessamento = TryGetInt("Configuracoes:PacotesProcessamento", 1000);
            dadosEmMemoria = TryGetInt("Configuracoes:DadosEmMemoria", 10000);
            threads = TryGetInt("Configuracoes:Threads", Math.Max(1, Environment.ProcessorCount / 2));
            candidato = _configurationDAO.PegarChave("Candidado") ?? string.Empty;
            logError = TryGetInt("Configuracoes:LogError", (int)ESalvarLog.NAO);
        }

        public async Task ProcessarLotePedagioAsync()
        {
            
            _erroCollectorDAO.CriarDiretorioLog();

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
                    totalSubLote++;

                    var dataReferencia = primeiro.DtCriacao.ToString("dd/MM/yyyy");
                    var numeroArquivo = GerarId.ObterProximoNumeroArquivo(dataReferencia);

                    var pedagio = new Pedagio
                    {
                        Candidato = candidato,
                        DataReferencia = dataReferencia,
                        NumeroArquivo = numeroArquivo,
                        Registros = new()
                    };

                    var resultPedagio = await _pedagioValidator.ValidateAsync(pedagio).ConfigureAwait(false);
                    if (!resultPedagio.IsValid)
                    {
                        if (logError == (int)ESalvarLog.SIM)
                            _erroCollectorDAO.Add(resultPedagio.Errors.Select(e => $"Arquivo {pedagio.NumeroArquivo} | Campo: {e.PropertyName} | Erro: {e.ErrorMessage} | Valor: {e.AttemptedValue} | Obj: {JsonSerializer.Serialize(pedagio)}"));
                        ultimaData = ultimo.DtCriacao;
                        continue;
                    }

                    var errosDoLote = await ProcessarSubLoteAsync(
                        _registroPedagioValidator,
                        threads,
                        numeroArquivo,
                        subLote,
                        pedagio
                    ).ConfigureAwait(false);

                    totalErro += errosDoLote;
                    
                    await _publicarDesafioDAO.PublicarRegistroPedagio(pedagio);

                    totalItemSubLote += pedagio.Registros.Count;
                    Terminal.Mensagem(
                        $"Lote {numeroArquivo} processado com {pedagio.Registros.Count} registros (Data {dataReferencia})",
                        "Processado: ",
                        ConsoleColor.Green
                    );

                    ultimaData = ultimo.DtCriacao;
                    await _erroCollectorDAO.SalvarEmDiscoAsync("ErrosPedagio.txt");
                }
            }

            Terminal.Mensagem(
                $"Processamento concluído — Lotes: {totalSubLote} | Registros processados: {totalItemSubLote} / {totalLoteGrande} | Error: {totalErro}.",
                "Finalizado: ",
                ConsoleColor.Green
            );

            if (logError == (int)ESalvarLog.SIM)
                Terminal.Mensagem($"Log salvo em {_erroCollectorDAO.PathLog()}", "LogError: ", ConsoleColor.Cyan);
        }

        private async Task<int> ProcessarSubLoteAsync(
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
                        CodigoPracaPedagio = item.CodigoPracaPedagio,
                        CodigoCabine = item.CodigoCabine.ToString(),
                        Instante = item.Instante,
                        Sentido = item.Sentido.ToString(),
                        TipoVeiculo = ((int)ETipoVeiculo.Comercial).ToString(),
                        Isento = item.Isento.ToString(),
                        Evasao = item.Evasao.ToString(),
                        TipoCobrancaEfetuada = item.TipoCobranca.ToString(),
                        ValorDevido = item.ValorDevido.ToString(),
                        ValorArrecadado = item.ValorArrecadado.ToString(),
                        MultiplicadorTarifa = "0"
                    };

                    var result = await registroPedagioValidator.ValidateAsync(registro).ConfigureAwait(false);
                    if (!result.IsValid)
                    {
                        Interlocked.Increment(ref erros);
                        if (logError == (int)ESalvarLog.SIM)
                            _erroCollectorDAO.Add(result.Errors.Select(e => $"Arquivo {pedagio.NumeroArquivo} | Campo: {e.PropertyName} | Erro: {e.ErrorMessage} | Valor: {e.AttemptedValue} | Obj: {JsonSerializer.Serialize(registro)}"));
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
            => int.TryParse(_configurationDAO.PegarChave(key), out var val) ? val : @default;
    }
}
