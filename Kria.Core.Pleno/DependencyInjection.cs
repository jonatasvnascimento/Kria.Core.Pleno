using Kria.Core.Pleno.Lib.BLL;
using Kria.Core.Pleno.Lib.Context;
using Kria.Core.Pleno.Lib.DAO;
using Kria.Core.Pleno.Lib.Interfaces.BLL;
using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;

namespace Kria.Core.Pleno
{
    public static class DependencyInjection
    {
        public static void Injection(this IServiceCollection services)
        {
            services.AddSingleton<IConfigurationDao, Configuration>();
            services.AddScoped<IPedagioDAO, PedagioDAO>();
            services.AddScoped<IPedagioBLL, PedagioBLL>();
            services.AddScoped<IMongoDbContext, MongoDbContext>();
        }
    }
}
