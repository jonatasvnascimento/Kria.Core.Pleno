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
            var transacoes = _pedagioDAO.ObterTodos().ToList();
            int tamanhoPacote = int.TryParse(_configurationDao.PegarChave("Configuracoes:Pacotes"), out var pacotes) ? pacotes : 1000;
            string Candidado = _configurationDao.PegarChave("Candidado") ?? string.Empty;
            int numeroPacote = 0;

            foreach (var pacote in transacoes.Chunk(tamanhoPacote))
            {
                numeroPacote++;
                _logger.LogInformation("Processando Pacote: {numeroPacote}", numeroPacote);

                await Parallel.ForEachAsync(pacote, new ParallelOptions { MaxDegreeOfParallelism = 1 }, async (item, _) =>
                {
                    Pedagio pedagio = new()
                    {
                        Candidato = Candidado,
                        DataReferencia = item.DtCriacao.ToString("dd/mm/yyyy"),
                        NumeroArquivo = GerarId.ObterProximoNumeroArquivo(item.DtCriacao.ToString("dd/mm/yyyy")),
                    };

                    var result = pedagioValidator.ValidateAsync(pedagio);

                    if (!result.Result.IsValid)
                    {
                        var errosDetalhados = result.Result.Errors?
                             .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                             .ToList() ?? new List<string> { result.Result.ToString() };
                        var mensagem = string.Join(", ", errosDetalhados);
                        _logger.LogError("Erros de validação no Pedagio: {Erros}", mensagem);
                        return;
                    }

                    RegistroPedagio registroPedagio = new()
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

                    var resultRegistro = registroPedagioValidator.ValidateAsync(registroPedagio);

                    if (!resultRegistro.Result.IsValid)
                    {
                        var errosDetalhados = resultRegistro.Result.Errors?
                             .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                             .ToList() ?? new List<string> { resultRegistro.Result.ToString() };
                        var mensagem = string.Join(", ", errosDetalhados);
                        _logger.LogError("Erros de validação no RegistroPedagio: {Erros}", mensagem);
                        return;
                    }

                    pedagio.Registros.Add(registroPedagio);

                });

                
            }

        }
    }
}
