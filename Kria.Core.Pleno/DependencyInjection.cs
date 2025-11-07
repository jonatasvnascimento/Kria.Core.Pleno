using Kria.Core.Pleno.Lib.Interfaces.DAO;
using Kria.Core.Pleno.Lib.Ultils;

namespace Kria.Core.Pleno
{
    public static class DependencyInjection
    {
        public static void Injection(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationDao, Configuration>();
        }
    }
}
