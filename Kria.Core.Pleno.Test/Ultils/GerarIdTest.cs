using Kria.Core.Pleno.Lib.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Kria.Core.Pleno.Test.Ultils
{
    public class GerarIdTests
    {
        [Fact]
        public void ObterProximoNumeroArquivo_PrimeiraChamada_DeveRetornar1()
        {
            // Arrange
            string data = "01/01/2025";

            // Act
            var resultado = GerarId.ObterProximoNumeroArquivo(data);

            // Assert
            Assert.Equal(1, resultado);
        }

        [Fact]
        public void ObterProximoNumeroArquivo_MesmaData_DeveIncrementarSequencial()
        {
            // Arrange
            string data = "02/01/2025";

            // Act
            var primeiro = GerarId.ObterProximoNumeroArquivo(data);
            var segundo = GerarId.ObterProximoNumeroArquivo(data);
            var terceiro = GerarId.ObterProximoNumeroArquivo(data);

            // Assert
            Assert.Equal(1, primeiro);
            Assert.Equal(2, segundo);
            Assert.Equal(3, terceiro);
        }

        [Fact]
        public void ObterProximoNumeroArquivo_DatasDiferentes_DevemTerContadoresIndependentes()
        {
            // Arrange
            string data1 = "03/01/2025";
            string data2 = "04/01/2025";

            // Act
            var idData1_Primeiro = GerarId.ObterProximoNumeroArquivo(data1);
            var idData1_Segundo = GerarId.ObterProximoNumeroArquivo(data1);
            var idData2_Primeiro = GerarId.ObterProximoNumeroArquivo(data2);

            // Assert
            Assert.Equal(1, idData1_Primeiro);
            Assert.Equal(2, idData1_Segundo);
            Assert.Equal(1, idData2_Primeiro);
        }

        [Fact]
        public void ObterProximoNumeroArquivo_MuitasChamadas_DeveManterSequenciaCorreta()
        {
            // Arrange
            string data = "05/01/2025";
            int totalChamadas = 100;

            // Act
            int ultimo = 0;
            for (int i = 0; i < totalChamadas; i++)
            {
                ultimo = GerarId.ObterProximoNumeroArquivo(data);
            }

            // Assert
            Assert.Equal(totalChamadas, ultimo);
        }
    }
}
