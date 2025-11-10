using Kria.Core.Pleno.Lib.BLL;
using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.DAO;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Kria.Core.Pleno.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Kria.Core.Pleno.Test
{
    public static class TestDependencyInjection
    {
        public static ServiceProvider BuildTestServices()
        {
            var services = new ServiceCollection();

            var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

            services.AddSingleton<IConfiguration>(config);
            services.AddSingleton<IConfigurationDAO, Configuration>();

            var mockConfig = new Mock<IConfigurationDAO>();
            services.AddSingleton(mockConfig.Object);

            services.AddScoped<IPedagioDAO, PedagioDAO>();
            services.AddScoped<IPedagioBLL, PedagioBLL>();
            services.AddScoped<IMongoDbContext, MongoDbContext>();
            services.AddScoped<IErroCollectorDAO, ErroCollector>();
            services.AddScoped<IPublicarDesafioDAO, PublicarDesafioDAO>();


            services.AddSingleton<GlobalErrorHandler>();
            services.AddScoped<PedagioValidator>();
            services.AddScoped<RegistroPedagioValidator>();
            services.AddScoped<HttpClient>();

            return services.BuildServiceProvider();
        }
    }
}
