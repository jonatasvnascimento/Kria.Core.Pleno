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
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async void ProcessarLotePedagio()
        {
            int pacotesProcessamento = int.TryParse(_configurationDao.PegarChave("Configuracoes:PacotesProcessamento"), out var p) ? p : 1000;
            int dadosEmMemoria = int.TryParse(_configurationDao.PegarChave("Configuracoes:DadosEmMemoria"), out var m) ? m : 10000;
            int threads = int.TryParse(_configurationDao.PegarChave("Configuracoes:Threads"), out var t) ? t : Environment.ProcessorCount / 2;
            string candidato = _configurationDao.PegarChave("Candidado") ?? string.Empty;

            DateTime? ultimaData = null;
            int numeroArquivo = 0;
            int totalLoteGrande = 0;
            int totalSubLote = 0;
            int totalItemSubLote = 0;
            int totalErro = 0;


            while (true)
            {
                var loteGrande = _pedagioDAO.ObterLote(ultimaData, dadosEmMemoria).ToList();
                if (!loteGrande.Any()) break;

                foreach (var subLote in loteGrande.Chunk(pacotesProcessamento))
                {
                    var dataReferencia = subLote.First().DtCriacao.ToString("dd/MM/yyyy");
                    numeroArquivo = GerarId.ObterProximoNumeroArquivo(dataReferencia);

                    Pedagio pedagio = new()
                    {
                        Candidato = candidato,
                        DataReferencia = dataReferencia,
                        NumeroArquivo = numeroArquivo,
                        Registros = new()
                    };
                    totalSubLote++;
                    totalLoteGrande += subLote.Count();

                    await Parallel.ForEachAsync(subLote, new ParallelOptions { MaxDegreeOfParallelism = threads }, async (item, _) =>
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
                            totalErro++;
                            var erros = string.Join(", ", result.Errors.Select(e => $"Arquivo: {numeroArquivo} {e.PropertyName}: {e.ErrorMessage}: Valor:{e.AttemptedValue}"));
                            return;
                        }

                        lock (pedagio.Registros)
                        {
                            pedagio.Registros.Add(registro);
                        }
                    });

                    var resultPedagio = await pedagioValidator.ValidateAsync(pedagio);
                    if (!resultPedagio.IsValid)
                    {
                        var erros = string.Join(", ", resultPedagio.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));
                        
                        ultimaData = subLote.Last().DtCriacao;
                        continue;
                    }

                    // await _pedagioDAO.SalvarPedagioAsync(pedagio);
                    totalItemSubLote += pedagio.Registros.Count;
                    Terminal.Mensagem($"Lote {numeroArquivo} processado com {pedagio.Registros.Count} registros (Data {dataReferencia})", "Processado: ", ConsoleColor.Green);

                    ultimaData = subLote.Last().DtCriacao;
                }

            }
            Terminal.Mensagem($"Processamento concluído — Lotes: {totalSubLote} | Registros processados: {totalItemSubLote} / {totalLoteGrande} | Error: {totalErro}.", "Finalizado: ", ConsoleColor.Green);
        }

    }
}
