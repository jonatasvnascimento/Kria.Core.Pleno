using Kria.Core.Pleno.Lib.Entidades;
using Kria.Core.Pleno.Lib.Validators;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using FluentValidation.TestHelper;

namespace Kria.Core.Pleno.Test.Validators
{
    public class PedagioValidatorTest
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly PedagioValidator _validator;

        public PedagioValidatorTest()
        {
            _serviceProvider = TestDependencyInjection.BuildTestServices();
            using var scope = _serviceProvider.CreateScope();
            _validator = scope.ServiceProvider.GetRequiredService<PedagioValidator>();
        }

        [Fact]
        public void Deve_Passar_Quando_TodosOsCamposForemValidos()
        {
            // Arrange
            var model = new Pedagio
            {
                Candidato = "João Silva",
                DataReferencia = DateTime.Now.ToString(),
                NumeroArquivo = 123
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Deve_Falhar_Quando_Candidato_ForNulo()
        {
            // Arrange
            var model = new Pedagio
            {
                Candidato = null!,
                DataReferencia = DateTime.Now.ToString(),
                NumeroArquivo = 10
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.Candidato)
                  .WithErrorMessage("O campo é obrigatório.");
        }

        [Fact]
        public void Deve_Falhar_Quando_DataReferencia_ForPadrao()
        {
            // Arrange
            var model = new Pedagio
            {
                Candidato = "José",
                DataReferencia = default!,
                NumeroArquivo = 99
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.DataReferencia)
                  .WithErrorMessage("O campo é obrigatório.");
        }

        [Fact]
        public void Deve_Falhar_Quando_NumeroArquivo_ForZero()
        {
            // Arrange
            var model = new Pedagio
            {
                Candidato = "Maria",
                DataReferencia = DateTime.Now.ToString(),
                NumeroArquivo = 0
            };

            // Act
            var result = _validator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(p => p.NumeroArquivo)
                  .WithErrorMessage("O campo é obrigatório.");
        }
    }
}
