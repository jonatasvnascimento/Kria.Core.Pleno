using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.DependencyInjection;

namespace Kria.Core.Pleno.Test.Ultils
{
    public class ErroCollectorTest : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly IErroCollectorDAO _erroCollector;

        private const string _logDir = "Logs";
        private const string _logFile = "Logs/test_log.txt";

        public ErroCollectorTest()
        {
            _serviceProvider = TestDependencyInjection.BuildTestServices();
            using var scope = _serviceProvider.CreateScope();
            _erroCollector = scope.ServiceProvider.GetRequiredService<IErroCollectorDAO>();

            // limpa logs antes de cada teste
            if (Directory.Exists(_logDir))
                Directory.Delete(_logDir, true);
        }

        [Fact]
        public void Add_DeveAdicionarUmErroNaLista()
        {
            // Act
            _erroCollector.Add("Error");

            // Assert
            Assert.Equal(1, _erroCollector.CountErros());
        }

        [Fact]
        public void Add_DeveAdicionarVariosErrosNaLista()
        {
            // Arrange
            var mensagens = new List<string> { "Erro 1", "Erro 2", "Erro 3" };

            // Act
            _erroCollector.Add(mensagens);

            // Assert
            Assert.Equal(3, _erroCollector.CountErros());
        }

        [Fact]
        public void CriarDiretorioLog_DeveCriarPastaLogs()
        {
            // Act
            _erroCollector.CriarDiretorioLog();

            // Assert
            Assert.True(Directory.Exists(_logDir));
        }

        [Fact]
        public async Task SalvarEmDiscoAsync_DeveCriarArquivoComErros()
        {
            // Arrange
            _erroCollector.Add("Erro teste");
            _erroCollector.CriarDiretorioLog();

            // Act
            await _erroCollector.SalvarEmDiscoAsync("test_log.txt");

            // Assert
            Assert.True(File.Exists(_logFile));
            string conteudo = await File.ReadAllTextAsync(_logFile);
            Assert.Contains("Erro teste", conteudo);
        }

        [Fact]
        public async Task SalvarEmDiscoAsync_SemErrosNaoDeveCriarArquivo()
        {
            // Act
            await _erroCollector.SalvarEmDiscoAsync("vazio.txt");

            // Assert
            Assert.False(File.Exists("Logs/vazio.txt"));
        }

        [Fact]
        public async Task PathLog_DeveRetornarOCaminhoDoArquivo()
        {
            // Arrange
            _erroCollector.Add("Erro X");
            _erroCollector.CriarDiretorioLog();

            // Act
            await _erroCollector.SalvarEmDiscoAsync("teste_path.txt");
            var path = _erroCollector.PathLog();

            // Assert
            Assert.Equal("Logs/teste_path.txt", path);
        }

        public void Dispose()
        {
            if (Directory.Exists(_logDir))
                Directory.Delete(_logDir, true);

            _serviceProvider?.Dispose();
        }
    }
}
