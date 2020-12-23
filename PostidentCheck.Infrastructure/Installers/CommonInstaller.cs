using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Interfaces;
using Postident.Core.Common;
using Postident.Core.Entities;
using Postident.Infrastructure.Common;
using Postident.Infrastructure.Interfaces;
using Postident.Infrastructure.Services;

namespace Postident.Infrastructure.Installers
{
    public static class CommonInstaller
    {
        public static IServiceCollection SetupCommonDependencies(this IServiceCollection services,
            IConfiguration configuration)
        {
            StaticObjectPropertyFromFileFiller.PopulateStaticProperties<DefaultShipmentValuesFromFile>(typeof(DefaultShipmentValues), configuration, "DefaultShipmentValues");

            services.AddTransient<IOfflineDataPackValidationService<InfoPackWriteModel>, OfflineDataPackValidationService>();
            services.AddTransient<IInvalidValidationToWriteModelMapper<InfoPackWriteModel>, InvalidValidationToWriteModelMapper>();

            services.AddTransient<TestService>();

            return services;
        }
    }
}