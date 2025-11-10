using FluentAssertions;
using Kria.Core.Pleno.Lib.DAO;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.Integration
{
    public class PublicarDesafioDAOTests
    {
        [Fact(DisplayName = "PublicarRegistroPedagio deve retornar status 200 OK")]
        public async Task PublicarRegistroPedagio_DeveRetornarStatus200()
        {
            // Arrange
            var fakeHandler = new FakeHttpMessageHandler(HttpStatusCode.OK, "{\"ok\":true}");
            var httpClient = new HttpClient(fakeHandler);
            var configuration = new FakeConfigurationComApi();

            var dao = new PublicarDesafioDAO(configuration, httpClient);

            var registro = new Pedagio
            {
                Candidato = "Jonatas Viana",
                DataReferencia = DateTime.UtcNow.ToString("dd/MM/yyyy"),
                NumeroArquivo = 123,
                Registros =
                {
                    new RegistroPedagio
                    {
                        GUID = Guid.NewGuid().ToString(),
                        CodigoPracaPedagio = 101,
                        CodigoCabine = 2,
                        Instante = DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss"),
                        Sentido = 1,
                        TipoVeiculo = 2,
                        Isento = 2,
                        Evasao = 1,
                        TipoCobrancaEfetuada = 1,
                        ValorDevido = 10.50m,
                        ValorArrecadado = 10.50m,
                        MultiplicadorTarifa = 1.0m
                    }
                }
            };

            // Act
            await dao.PublicarRegistroPedagio(registro);

            // Assert
            fakeHandler.LastStatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    internal class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string _responseContent;
        public HttpStatusCode LastStatusCode { get; private set; }

        public FakeHttpMessageHandler(HttpStatusCode statusCode, string responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };

            LastStatusCode = response.StatusCode;
            return Task.FromResult(response);
        }
    }

    internal class FakeConfigurationComApi : IConfigurationDAO
    {
        public string PegarChave(string chave) => chave == "Api" ? "https://fake.api" : null;
    }
}
