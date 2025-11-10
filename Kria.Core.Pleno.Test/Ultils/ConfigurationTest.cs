using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.Ultils
{
    public class ConfigurationTest
    {
        private readonly IConfigurationDAO _configuration;
        private readonly ServiceProvider _serviceProvider;

        public ConfigurationTest()
        {
            _serviceProvider = TestDependencyInjection.BuildTestServices();
            using var scope = _serviceProvider.CreateScope();
            _configuration = scope.ServiceProvider.GetRequiredService<IConfigurationDAO>();
        }

        [Fact]
        public void PegarChave_ChaveExiste_DeveRetornarChaveDoAppssetings()
        {
            // Arrange
            string key = "Candidado";

            // Act
            var NomeCandidado = _configuration.PegarChave(key);

            // Assert
            Assert.NotNull(NomeCandidado);
            Assert.Equal("Jonatas Viana", NomeCandidado);
            Assert.False(string.IsNullOrEmpty(NomeCandidado));
        }

        [Fact]
        public void PegarChave_ChaveNaoExiste_DeveRetornarNull()
        {
            // Arrange
            string key = "Aleatorio";

            // Act
            var NomeCandidado = _configuration.PegarChave(key);

            // Assert
            Assert.Equal(null!, NomeCandidado);
            Assert.True(string.IsNullOrEmpty(NomeCandidado));
        }
    }
}
