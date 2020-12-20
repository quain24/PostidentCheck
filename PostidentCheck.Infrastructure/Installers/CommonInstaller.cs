using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postident.Infrastructure.Services;

namespace Postident.Infrastructure.Installers
{
    public static class CommonInstaller
    {
        public static IServiceCollection SetupCommonDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddTransient<TestService>();
            
            return services;
        }
    }
}