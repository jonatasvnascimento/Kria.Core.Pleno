using FluentAssertions;
using Kria.Core.Pleno.Lib.BLL;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Validators;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.Performance
{
    public class PedagioPerformanceTests
    {
        [Fact]
        public async Task ProcessarSubLoteAsync_DeveProcessarRegistros_ComPerformanceAceitavel()
        {
            // Arrange
            const int total = 10_000_000;
            const int threads = 8;
            var registroValidator = new RegistroPedagioValidator();

            // Cria 1 milhão de registros simulados
            var subLote = new TabTransacoes[total];
            var agora = DateTime.UtcNow;

            for (int i = 0; i < total; i++)
            {
                subLote[i] = new TabTransacoes
                {
                    _id = Guid.NewGuid().ToString(),
                    IdTransacao = i + 1,
                    DtCriacao = agora.AddMilliseconds(i),
                    CodigoPracaPedagio = "0197",
                    CodigoCabine = 2377,
                    Instante = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    Sentido = 1,
                    QuantidadeEixosVeiculo = 2,
                    Rodagem = 1,
                    Isento = 2,
                    MotivoIsencao = 2,
                    Evasao = 2,
                    EixoSuspenso = 2,
                    QuantidadeEixosSuspensos = 0,
                    TipoCobranca = 1,
                    Placa = "ABC1234",
                    LiberacaoCancela = 1,
                    ValorDevido = 15.75m,
                    ValorArrecadado = 15.75m,
                    CnpjAmap = "12345678000199",
                    MultiplicadorTarifa = 1.0f,
                    VeiculoCarregado = 1,
                    IdTag = "TAG123"
                };
            }

            var pedagio = new Pedagio
            {
                Candidato = "TesteCarga",
                DataReferencia = DateTime.Now.ToString("dd/MM/yyyy"),
                NumeroArquivo = 1,
                Registros = new List<RegistroPedagio>()
            };

            var mockConfig = new Mock<IConfigurationDAO>();
            var mockErro = new Mock<IErroCollectorDAO>();
            var mockPedagio = new Mock<IPedagioDAO>();
            var mockPub = new Mock<IPublicarDesafioDAO>();

            var bll = new PedagioBLL(
                mockPedagio.Object,
                mockConfig.Object,
                mockErro.Object,
                mockPub.Object,
                new PedagioValidator(),
                registroValidator
            );

            var metodo = typeof(PedagioBLL)
                .GetMethod("ProcessarSubLoteAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

            // Act
            var sw = Stopwatch.StartNew();

            var task = (Task<int>)metodo.Invoke(bll, new object[]
            {
                registroValidator, threads, 1, subLote, pedagio
            })!;

            var erros = await task;
            sw.Stop();

            // Assert
            pedagio.Registros.Count.Should().Be(total - erros);
            sw.Elapsed.Should().BeLessThan(TimeSpan.FromSeconds(30)); // ajuste conforme sua máquina
            Console.WriteLine($"⏱ Processados: {total:N0} registros em {sw.Elapsed.TotalSeconds:F2}s | Erros: {erros}");
        }
    }
}
