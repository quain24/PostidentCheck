using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postident.Infrastructure.Common;
using Postident.Infrastructure.Services;
using System.Linq;
using System.Reflection;

namespace Postident.Infrastructure.Installers
{
    public static class CommonInstaller
    {
        public static IServiceCollection SetupCommonDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            PopulateDefaultShipmentValues(configuration);

            services.AddTransient<TestService>();

            return services;
        }

        /// <summary>
        /// This will populate a static class <see cref="DefaultShipmentValues"/> with values from json config file.
        /// </summary>
        /// <param name="configuration">Configuration parameter from dependency injection mechanism</param>
        private static void PopulateDefaultShipmentValues(IConfiguration configuration)
        {
            var dhlSettingsFromFile = new DefaultShipmentValuesFromFile();
            configuration.Bind("DefaultShipmentValues", dhlSettingsFromFile);

            var sourceProperties = dhlSettingsFromFile.GetType().GetProperties();
            var destinationProperties = typeof(DefaultShipmentValues)
                .GetProperties(BindingFlags.Public | BindingFlags.Static);

            foreach (var prop in sourceProperties)
            {
                //Find matching property by name
                var destinationProp = destinationProperties
                    .Single(p => p.Name == prop.Name);

                //Set the static property value
                destinationProp.SetValue(null, prop.GetValue(dhlSettingsFromFile));
            }
        }
    }
}