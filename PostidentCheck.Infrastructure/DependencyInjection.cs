using KeePass;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postident.Infrastructure.Installers;

namespace Postident.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
        {
            services
                .SetupDatabaseAccess(config.GetConnectionString("DilosDEV"))
                .SetupDHLServices(config)
                .SetupCommonDependencies(config)
                .SetupKeePassServices(config);

            return services;
        }
    }
}