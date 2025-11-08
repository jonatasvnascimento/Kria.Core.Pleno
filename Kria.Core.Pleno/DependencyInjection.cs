using Kria.Core.Pleno.Lib.BLL;
using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.DAO;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;
using Kria.Core.Pleno.Lib.Validators;
using Kria.Core.Pleno.Middleware;

namespace Kria.Core.Pleno
{
    public static class DependencyInjection
    {
        public static void Injection(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationDao, Configuration>();
            services.AddSingleton<GlobalErrorHandler>();
            services.AddScoped<IPedagioDAO, PedagioDAO>();
            services.AddScoped<IPedagioBLL, PedagioBLL>();
            services.AddScoped<IMongoDbContext, MongoDbContext>();
            services.AddScoped<PedagioValidator>();
            services.AddScoped<RegistroPedagioValidator>();
            services.AddScoped<ErroCollector>();
        }
    }
}
