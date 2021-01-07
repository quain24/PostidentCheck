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

            services.SetupSetting(configuration, dhlSettingsFromFile);

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

            services.AddTransient<IValidationRequestXmlBuilder, ValidationRequestXmlBuilder>();
            services.AddSingleton<IValidationRequestXmlBuilderFactory, ValidationRequestXmlBuilderFactory>();
            services.AddTransient<ISingleShipmentBuilder, SingleShipmentBuilder>();

            services.AddTransient<IDhlApiService, DhlOnlineValidationService>();
            services.AddTransient<ICarrierApiServiceResponseDeserializer<DhlMainResponseDto>, DhlResponseDeserializer>
            (srv => new DhlResponseDeserializer(serviceName, srv.GetRequiredService<ILogger<DhlResponseDeserializer>>()));

            return services;
        }

        /// <summary>
        /// This method will choose appropriate source of logins and passwords. If this data is set in json file, then<br/>
        /// KeePass service will not be called - useful when debugging on local machine.<br/>
        /// Otherwise normal KeePass service will be used.
        /// </summary>
        private static IServiceCollection SetupSetting(this IServiceCollection services, IConfiguration configuration, DhlSettingsFromAppsettings settingsFromFile)
        {
            var logins = new DhlOfflineSettings.AppSet();
            configuration.Bind("KeePassOfflineStore", logins);

            if (string.IsNullOrWhiteSpace(logins.Login) || string.IsNullOrWhiteSpace(logins.XmlLogin))
            {
                services.AddSingleton<IDhlSettings, DhlSettingsFromPasswordServer>(provider =>
                new DhlSettingsFromPasswordServer(provider.GetRequiredService<IKeePassService>(), settingsFromFile));
            }
            else
            {
                services.AddSingleton<IDhlSettings, DhlOfflineSettings>(_ =>
                new DhlOfflineSettings(settingsFromFile, logins.Login, logins.Password, logins.XmlLogin, logins.XmlPassword));
            }

            return services;
        }
    }
}