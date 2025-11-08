using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Middleware;

namespace Kria.Core.Pleno
{
    public class Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory,
        GlobalErrorHandler errorHandlingMiddleware,
        IConfigurationDao configurationDao
    ) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
        private readonly GlobalErrorHandler _errorHandlingMiddleware = errorHandlingMiddleware;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var pedagioBLL = scope.ServiceProvider.GetRequiredService<IPedagioBLL>();

            while (!stoppingToken.IsCancellationRequested)
            {
                await _errorHandlingMiddleware.HandleAsync(async () =>
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Serviço Rodando: {time}", DateTimeOffset.Now);

                    var nextRun = int.TryParse(configurationDao.PegarChave("Configuracoes:Execucao"), out var execucao) ? execucao : 2;
                    await Task.Delay(TimeSpan.FromSeconds(nextRun), stoppingToken);
                    _ = pedagioBLL.ProcessarLotePedagioAsync();
                });
            }
        }
    }
}
