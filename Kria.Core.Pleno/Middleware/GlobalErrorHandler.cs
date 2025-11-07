using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace Kria.Core.Pleno.Middleware
{
    public class GlobalErrorHandler
    {
        private readonly ILogger<GlobalErrorHandler> _logger;

        public GlobalErrorHandler(ILogger<GlobalErrorHandler> logger)
        {
            _logger = logger;
        }
        public async Task HandleAsync(Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errosDetalhados = ex.Errors?
                     .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                     .ToList() ?? new List<string> { ex.Message };

                var mensagem = string.Join(", ", errosDetalhados);

                _logger.LogError("Erros de validação: {Erros}", mensagem);

                await File.AppendAllTextAsync("erros_validacao.txt",
                    $"""
                    ============================================
                    Data: {DateTime.Now}
                    Tipo: {ex.GetType().FullName}
                    Erros:
                    {string.Join(Environment.NewLine, errosDetalhados)}
                    ============================================
                    """);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no Worker Service: {Mensagem}", ex.Message);
                await File.AppendAllTextAsync("erros_gerais.txt",
                    $"""
                    ============================================
                    Data: {DateTime.Now}
                    Mensagem: {ex.Message}
                    Tipo: {ex.GetType().FullName}
                    StackTrace:
                    {ex.StackTrace}
                    ============================================
                    """);
            }
        }
    }
}
