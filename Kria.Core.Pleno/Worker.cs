using Kria.Core.Pleno.Lib.Interfaces.BLL;

namespace Kria.Core.Pleno
{
    public class Worker(
        ILogger<Worker> logger,
        IServiceScopeFactory scopeFactory
    ) : BackgroundService
    {
        private readonly ILogger<Worker> _logger = logger;
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
                DadosTeste();
                await Task.Delay(10000, stoppingToken);
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
