using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.Enum;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kria.Core.Pleno.Lib.BLL
{
    public class PedagioBLL(
        IPedagioDAO pedagioDAO,
        IConfigurationDao configurationDao,
        ILogger<PedagioBLL> logger,
        PedagioValidator pedagioValidator,
        RegistroPedagioValidator registroPedagioValidator
        ) : IPedagioBLL
    {
        private readonly IPedagioDAO _pedagioDAO = pedagioDAO;
        private readonly IConfigurationDao _configurationDao = configurationDao;
        private readonly ILogger<PedagioBLL> _logger = logger;

        public async void ProcessarLotePedagio()
        {
            int tamanhoPacote = int.TryParse(_configurationDao.PegarChave("Configuracoes:Pacotes"), out var pacotes) ? pacotes : 1000;
            int threads = int.TryParse(_configurationDao.PegarChave("Configuracoes:Threads"), out var t) ? t : Environment.ProcessorCount / 2;
            string candidato = _configurationDao.PegarChave("Candidado") ?? string.Empty;

            DateTime? ultimaData = null;
            int numeroArquivo = 0;
            var totalLote = 0;


            while (true)
            {
                var lote = _pedagioDAO.ObterLote(ultimaData, tamanhoPacote).ToList();
                if (!lote.Any()) break;

                var dataReferencia = lote.First().DtCriacao.ToString("dd/MM/yyyy");
                numeroArquivo = GerarId.ObterProximoNumeroArquivo(dataReferencia);

                Pedagio pedagio = new()
                {
                    Candidato = candidato,
                    DataReferencia = dataReferencia,
                    NumeroArquivo = numeroArquivo,
                    Registros = new()
                };
                totalLote++;

                await Parallel.ForEachAsync(lote, new ParallelOptions { MaxDegreeOfParallelism = threads }, async (item, _) =>
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

                    var result = await registroPedagioValidator.ValidateAsync(registro);
                    if (!result.IsValid)
                    {
                        var erros = string.Join(", ", result.Errors.Select(e => $"Arquivo: {numeroArquivo} {e.PropertyName}: {e.ErrorMessage}: Valor:{e.AttemptedValue}"));
                        //_logger.LogError("Erro de validação no registro: {Erros}", erros);
                        return;
                    }

                    lock (pedagio.Registros)
                    {
                        pedagio.Registros.Add(registro);
                    }

                    _logger.LogDebug("Registro processado: {GUID}", registro.GUID);
                });

                var resultPedagio = await pedagioValidator.ValidateAsync(pedagio);
                if (!resultPedagio.IsValid)
                {
                    var erros = string.Join(", ", resultPedagio.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                    //_logger.LogError("Erros de validação no Pedagio: {Erros}", erros);
                    ultimaData = lote.Last().DtCriacao;
                    continue;
                }

                // await _pedagioDAO.SalvarPedagioAsync(pedagio);

                _logger.LogInformation("Lote {NumeroArquivo} processado com {Qtd} registros (Data {Data})",
                    numeroArquivo, pedagio.Registros.Count, dataReferencia);

                ultimaData = lote.Last().DtCriacao;
            }

            _logger.LogInformation("Processamento concluído com {NumLotes} lotes.", totalLote);

        }

    }
}
