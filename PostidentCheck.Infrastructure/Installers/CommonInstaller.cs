using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Interfaces;
using Postident.Core.Common;
using Postident.Core.Entities;
using Postident.Infrastructure.Common;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Services;
using System;
using System.Collections.Generic;

namespace Postident.Infrastructure.Installers
{
    public static class CommonInstaller
    {
        public static IServiceCollection SetupCommonDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            StaticObjectPropertyFromFileFiller.PopulateStaticProperties<DefaultShipmentValuesFromFile>(typeof(DefaultShipmentValues), configuration, "DefaultShipmentValues");

            services.AddDefaultNamingMap(configuration);
            services.AddTransient<IOfflineDataPackValidationService<InfoPackWriteModel>, OfflineDataPackValidationService>();
            services.AddTransient<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>, InvalidValidationToWriteModelMapper>();

            services.AddTransient<TestService>();

            return services;
        }

        /// <summary>
        /// Registers default naming map for api services.
        /// Provides default naming for when our database uses some synonyms and api requires precise data;
        /// </summary>
        private static IServiceCollection AddDefaultNamingMap(this IServiceCollection services, IConfiguration configuration)
        {
            var mapObject = new { Map = new Dictionary<string, HashSet<string>>(StringComparer.InvariantCultureIgnoreCase) };
            configuration.Bind("DefaultNamingMap", mapObject);

            services.AddSingleton(_ => new DefaultNamingMap(mapObject.Map));

            return services;
        }
    }
}