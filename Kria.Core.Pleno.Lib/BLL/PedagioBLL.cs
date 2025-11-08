using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.Enum;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Kria.Core.Pleno.Lib.BLL
{
    public class PedagioBLL : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO;
        private readonly IConfigurationDao _configurationDao;
        private readonly PedagioValidator _pedagioValidator;
        private readonly RegistroPedagioValidator _registroPedagioValidator;
        private readonly ILogger<PedagioBLL> _logger;

        public PedagioBLL(
            IPedagioDAO pedagioDAO,
            IConfigurationDao configurationDao,
            PedagioValidator pedagioValidator,
            RegistroPedagioValidator registroPedagioValidator,
            ILogger<PedagioBLL> logger)
        {
            _pedagioDAO = pedagioDAO;
            _configurationDao = configurationDao;
            _pedagioValidator = pedagioValidator;
            _registroPedagioValidator = registroPedagioValidator;
            _logger = logger;
        }

        public async Task ProcessarLotePedagioAsync(CancellationToken cancellationToken = default)
        {
            int pacotesProcessamento = GetConfigInt("Configuracoes:PacotesProcessamento", 1000);
            int dadosEmMemoria = GetConfigInt("Configuracoes:DadosEmMemoria", 10000);
            int threads = GetConfigInt("Configuracoes:Threads", Environment.ProcessorCount / 2);
            string candidato = _configurationDao.PegarChave("Candidado") ?? string.Empty;

            var erros = new ErroCollector();
            var stats = new ProcessamentoStats();

            DateTime? ultimaData = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                var loteGrande = _pedagioDAO.ObterLote(ultimaData, dadosEmMemoria).ToList();
                if (!loteGrande.Any()) break;

                foreach (var subLote in loteGrande.Chunk(pacotesProcessamento))
                {
                    ultimaData = subLote.Last().DtCriacao;
                    var dataRef = subLote.First().DtCriacao.ToString("dd/MM/yyyy");
                    int numeroArquivo = GerarId.ObterProximoNumeroArquivo(dataRef);

                    var pedagio = new Pedagio
                    {
                        Candidato = candidato,
                        DataReferencia = dataRef,
                        NumeroArquivo = numeroArquivo,
                        Registros = new()
                    };
                    
                    var resultPedagio = await _pedagioValidator.ValidateAsync(pedagio);
                    if (!resultPedagio.IsValid)
                    {
                        erros.Add(resultPedagio.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                        continue;
                    }

                    await ProcessarSubLoteAsync(subLote, pedagio, threads, erros, stats, cancellationToken);
                    

                    stats.TotalLotes++;
                    stats.TotalRegistrosProcessados += pedagio.Registros.Count;

                    Terminal.Mensagem(
                        $"Lote {numeroArquivo} processado com {pedagio.Registros.Count} registros (Data {dataRef})",
                        "Processado: ",
                        ConsoleColor.Green);
                }
            }

            await erros.SalvarEmDiscoAsync("ErrosPedagio.txt");
            Terminal.Mensagem(
                $"Processamento concluído — Lotes: {stats.TotalLotes} | Registros: {stats.TotalRegistrosProcessados} | Erros: {erros.Count}",
                "Finalizado: ",
                ConsoleColor.Green);
        }

        private async Task ProcessarSubLoteAsync(
            IEnumerable<TabTransacoes> subLote,
            Pedagio pedagio,
            int threads,
            ErroCollector erros,
            ProcessamentoStats stats,
            CancellationToken token)
        {
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = threads,
                CancellationToken = token
            };

            await Parallel.ForEachAsync(subLote, options, async (item, _) =>
            {
                try
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

                    var result = await _registroPedagioValidator.ValidateAsync(registro);
                    if (!result.IsValid)
                    {
                        erros.Add(result.Errors.Select(e =>
                            $"Arquivo {pedagio.NumeroArquivo} | Campo: {e.PropertyName} | Erro: {e.ErrorMessage} | ValorError: {e.AttemptedValue}"));
                        return;
                    }

                    lock (pedagio.Registros)
                        pedagio.Registros.Add(registro);
                }
                catch (Exception ex)
                {
                    erros.Add($"Exceção: {ex.Message}");
                }
            });
        }

        private int GetConfigInt(string chave, int padrao)
        {
            return int.TryParse(_configurationDao.PegarChave(chave), out var valor) ? valor : padrao;
        }
    }

}
