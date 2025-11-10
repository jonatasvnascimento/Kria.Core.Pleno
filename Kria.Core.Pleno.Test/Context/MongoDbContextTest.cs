using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.Context
{
    public class MongoDbContextTest
    {
        private readonly IConfigurationDAO _configuration;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<IConfigurationDAO> _mockConfig;

        public MongoDbContextTest()
        {
            _serviceProvider = TestDependencyInjection.BuildTestServices();
            using var scope = _serviceProvider.CreateScope();
            _configuration = scope.ServiceProvider.GetRequiredService<IConfigurationDAO>();
            _mockConfig = new Mock<IConfigurationDAO>();
        }

        [Fact]
        public void Construtor_DeveCriarInstanciaCorretamente_QuandoConfiguracaoValida()
        {
            // Arrange
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:DefaultConnection"))
                       .Returns("mongodb://localhost:27017");
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:Database"))
                       .Returns("BancoTeste");

            // Act
            var context = new MongoDbContext(_mockConfig.Object);

            // Assert
            Assert.NotNull(context);

            // Act
            var contexts = new MongoDbContext(_mockConfig.Object);

            // Assert
            Assert.NotNull(contexts);
        }

        [Fact]
        public void GetCollection_DeveRetornarColecaoCorreta()
        {
            // Arrange
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:DefaultConnection"))
                       .Returns("mongodb://localhost:27017");
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:Database"))
                       .Returns("BancoTeste");

            var context = new MongoDbContext(_mockConfig.Object);

            // Act
            var collection = context.GetCollection<BsonDocument>("MinhaColecao");

            // Assert
            Assert.NotNull(collection);
            Assert.IsAssignableFrom<IMongoCollection<BsonDocument>>(collection);
        }

        [Fact]
        public void Construtor_DeveLancarErro_QuandoConnectionStringInvalida()
        {
            // Arrange
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:DefaultConnection"))
                       .Returns(string.Empty);
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:Database"))
                       .Returns("BancoTeste");

            // Act & Assert
            Assert.Throws<MongoConfigurationException>(() =>
            {
                var context = new MongoDbContext(_mockConfig.Object);
            });
        }

        [Fact]
        public void Construtor_DeveLancarErro_QuandoDatabaseNaoInformado()
        {
            // Arrange
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:DefaultConnection"))
                       .Returns(string.Empty);
            _mockConfig.Setup(c => c.PegarChave("ConnectionStrings:Database"))
                       .Returns(string.Empty);

            // Act & Assert
            var ex = Assert.Throws<MongoConfigurationException>(() =>
            {
                var context = new MongoDbContext(_mockConfig.Object);
            });

            Assert.Equal("String de conexão do Banco não configurada no appsettings", ex.Message);
        }
    }
}
