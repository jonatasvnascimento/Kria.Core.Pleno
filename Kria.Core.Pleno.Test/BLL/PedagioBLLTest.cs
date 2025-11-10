using FluentAssertions;
using FluentValidation.Results;
using Kria.Core.Pleno.Lib.BLL;
using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Entidades.Enum;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.BLL
{
    public class PedagioBLLTests
    {
        private readonly Mock<IPedagioDAO> _pedagioDaoMock;
        private readonly Mock<IConfigurationDAO> _configDaoMock;
        private readonly Mock<IErroCollectorDAO> _erroCollectorMock;
        private readonly Mock<IPublicarDesafioDAO> _publicarMock;
        private readonly PedagioValidator _pedagioValidator;
        private readonly RegistroPedagioValidator _registroValidator;

        public PedagioBLLTests()
        {
            _pedagioDaoMock = new Mock<IPedagioDAO>();
            _configDaoMock = new Mock<IConfigurationDAO>();
            _erroCollectorMock = new Mock<IErroCollectorDAO>();
            _publicarMock = new Mock<IPublicarDesafioDAO>();

            _pedagioValidator = new PedagioValidator();
            _registroValidator = new RegistroPedagioValidator();

            // Configurações padrão
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:PacotesProcessamento")).Returns("1000");
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:DadosEmMemoria")).Returns("5000");
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:Threads")).Returns("2");
            _configDaoMock.Setup(x => x.PegarChave("Candidado")).Returns("Jonatas");
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:LogError")).Returns(((int)ESalvarLog.NAO).ToString());
        }

        private PedagioBLL CriarBLL()
        {
            return new PedagioBLL(
                _pedagioDaoMock.Object,
                _configDaoMock.Object,
                _erroCollectorMock.Object,
                _publicarMock.Object,
                _pedagioValidator,
                _registroValidator
            );
        }

        [Fact]
        public void Construtor_DeveInicializarComValoresPadrao_QuandoChavesInexistentes()
        {
            // Arrange
            _configDaoMock.Setup(x => x.PegarChave(It.IsAny<string>())).Returns((string)null!);

            // Act
            var bll = CriarBLL();

            // Assert
            bll.Should().NotBeNull();
        }

        [Fact]
        public async Task ProcessarLotePedagioAsync_DeveParar_QuandoNaoExistemLotes()
        {
            // Arrange
            _pedagioDaoMock.Setup(x => x.ObterLote(null, It.IsAny<int>()))
                           .Returns(new List<TabTransacoes>());

            var bll = CriarBLL();

            // Act
            await bll.ProcessarLotePedagioAsync();

            // Assert
            _publicarMock.Verify(x => x.PublicarRegistroPedagio(It.IsAny<Pedagio>()), Times.Never);
        }

        [Fact]
        public async Task ProcessarLotePedagioAsync_DevePublicarPedagio_QuandoValido()
        {
            // Arrange
            var transacoes = new[]
            {
                new TabTransacoes
                {
                    DtCriacao = DateTime.Now,
                    CodigoPracaPedagio = "100",
                    CodigoCabine = 1,
                    Instante = "2024-01-01",
                    Sentido = 1,
                    TipoCobranca = 1,
                    Isento = 1,
                    Evasao = 1,
                    ValorDevido = 10,
                    ValorArrecadado = 10
                }
            };

            _pedagioDaoMock.Setup(x => x.ObterLote(null, It.IsAny<int>()))
                           .Returns(transacoes);

            _publicarMock.Setup(x => x.PublicarRegistroPedagio(It.IsAny<Pedagio>()))
                         .Returns(Task.CompletedTask);

            var bll = CriarBLL();

            // Act
            await bll.ProcessarLotePedagioAsync();

            // Assert
            _publicarMock.Verify(x => x.PublicarRegistroPedagio(It.IsAny<Pedagio>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ProcessarLotePedagioAsync_DeveSalvarErros_QuandoValidacaoFalhar()
        {
            // Arrange
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:LogError"))
                          .Returns(((int)ESalvarLog.SIM).ToString());

            var transacoes = new[]
            {
                new TabTransacoes
                {
                    DtCriacao = DateTime.Now,
                    CodigoPracaPedagio = "A", // inválido
                    CodigoCabine = 0,
                    Instante = "",
                    Sentido = 0,
                    TipoCobranca = 0,
                    Isento = 0,
                    Evasao = 0,
                    ValorDevido = 0,
                    ValorArrecadado = 0
                }
            };

            _pedagioDaoMock.Setup(x => x.ObterLote(null, It.IsAny<int>()))
                           .Returns(transacoes);

            var bll = CriarBLL();

            // Act
            await bll.ProcessarLotePedagioAsync();

            // Assert
            _erroCollectorMock.Verify(x => x.Add(It.IsAny<IEnumerable<string>>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ProcessarLotePedagioAsync_DeveCriarDiretorioLog_AoIniciar()
        {
            // Arrange
            _pedagioDaoMock.Setup(x => x.ObterLote(null, It.IsAny<int>()))
                           .Returns(new List<TabTransacoes>());

            var bll = CriarBLL();

            // Act
            await bll.ProcessarLotePedagioAsync();

            // Assert
            _erroCollectorMock.Verify(x => x.CriarDiretorioLog(), Times.Once);
        }

        [Fact]
        public async Task ProcessarSubLoteAsync_DeveAdicionarRegistrosValidos()
        {
            // Arrange
            var subLote = new[]
            {
                new TabTransacoes
                {
                    DtCriacao = DateTime.Now,
                    CodigoPracaPedagio = "100",
                    CodigoCabine = 10,
                    Instante = "2024-01-01",
                    Sentido = 1,
                    TipoCobranca = 1,
                    Isento = 1,
                    Evasao = 1,
                    ValorDevido = 10,
                    ValorArrecadado = 10
                }
            };

            var pedagio = new Pedagio
            {
                Candidato = "Jonatas",
                DataReferencia = "01/01/2024",
                NumeroArquivo = 1,
                Registros = new List<RegistroPedagio>()
            };

            var bll = CriarBLL();

            var metodo = typeof(PedagioBLL)
                .GetMethod("ProcessarSubLoteAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

            // Act
            var task = (Task<int>)metodo.Invoke(bll, new object[]
            {
                _registroValidator, 2, 1, subLote, pedagio
            })!;
            var erros = await task;

            // Assert
            erros.Should().BeGreaterThanOrEqualTo(0);
            pedagio.Registros.Should().NotBeEmpty();
        }

        [Fact]
        public void TryGetInt_DeveRetornarDefault_QuandoNaoForNumero()
        {
            // Arrange
            _configDaoMock.Setup(x => x.PegarChave("Configuracoes:Threads")).Returns("abc");
            var bll = CriarBLL();

            var metodo = typeof(PedagioBLL)
                .GetMethod("TryGetInt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;

            // Act
            var valor = (int)metodo.Invoke(bll, new object[] { "Configuracoes:Threads", 5 })!;

            // Assert
            valor.Should().Be(5);
        }
    }
}
