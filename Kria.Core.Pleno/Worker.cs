using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Middleware;

namespace Kria.Core.Pleno
{
    public class Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        GlobalErrorHandler errorHandlingMiddleware
    ) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly GlobalErrorHandler _errorHandlingMiddleware = errorHandlingMiddleware;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _errorHandlingMiddleware.HandleAsync(async () =>
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                    {
                        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    }
                    await Task.Delay(2000, stoppingToken);
                    DadosTeste();
                });
            }
        }
       
        private void DadosTeste()
        {

            using var scope = _scopeFactory.CreateScope();
            var pedagioBLL = scope.ServiceProvider.GetRequiredService<IPedagioBLL>();

            Console.WriteLine("Listando Pedágios:");
            var pedagios = pedagioBLL.ObterTodos().ToList();
            foreach (var pedagio in pedagios)
            {
                Console.WriteLine($"- {pedagio.IdTransacao}");
            }
        }
    }
}
