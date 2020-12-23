using KeePass;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Postident.Application.Common.Interfaces;
using Postident.Application.DHL;
using Postident.Application.DHL.Interfaces;
using Postident.Infrastructure.Interfaces.DHL;
using Postident.Infrastructure.Policies;
using Postident.Infrastructure.Services.DHL;
using System;
using System.Net.Http.Headers;

namespace Postident.Infrastructure.Installers
{
    /// <summary>
    /// All services, dependencies, configuration etc connected do DHL PostIdent service are contained in this class,
    /// to be used with .NET DI container
    /// </summary>
    internal static class DhlServicesInstaller
    {
        /// <summary>
        /// <inheritdoc cref="DhlServicesInstaller"/>
        /// </summary>
        internal static IServiceCollection SetupDHLServices(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceName = "DHL PostIdent";

            var dhlSettingsFromFile = new DhlSettingsFromAppsettings();
            configuration.Bind("DHL", dhlSettingsFromFile);
            services.AddSingleton<IDhlSettings, DhlSettingsFromPasswordServer>(provider => new DhlSettingsFromPasswordServer(provider.GetRequiredService<IKeePassService>(), dhlSettingsFromFile));

            services.AddHttpClient(serviceName, client =>
                {
                    client.BaseAddress = new Uri(dhlSettingsFromFile.BaseAddress);
                    client.Timeout = TimeSpan.FromSeconds(30);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/xml"));
                    client.DefaultRequestHeaders.Add("SOAPAction", "\"urn:validateShipment\"");
                })
                .AddPolicyHandler(HttpClientPolicies.FallbackForHttpRequestException(serviceName))
                .AddPolicyHandler(HttpClientPolicies.WaitAndRetryAsyncPolicy(serviceName, 3)) // 3 retries, so 4 actual attempts
                .AddPolicyHandler(HttpClientPolicies.CircuitBreakerAsyncOneTimePolicy(serviceName, 10)); // actual attempts * how many messages can fail without a good between one between

            services.AddTransient<IDhlApiService, DhlPostidentService>();
            services.AddTransient<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>, DhlResponseDeserializer>
            (srv => new DhlResponseDeserializer(serviceName, srv.GetRequiredService<ILogger<DhlResponseDeserializer>>()));

            return services;
        }
    }
}