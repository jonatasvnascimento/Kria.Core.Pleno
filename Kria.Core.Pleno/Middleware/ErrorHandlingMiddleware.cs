using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Kria.Core.Pleno.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task ExecuteAsync(Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (FluentValidation.ValidationException ex)
            {
                _logger.LogWarning("Erro de validação: {Erros}", string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
                await File.AppendAllTextAsync("erros_validacao.txt", $"{DateTime.Now}: {ex.Message}\n");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no Worker Service");
                await File.AppendAllTextAsync("erros_gerais.txt", $"{DateTime.Now}: {ex.Message}\n");
            }
        }
    }
}
