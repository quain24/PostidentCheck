using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Interfaces;
using Postident.Infrastructure.Persistence;
using System;

namespace Postident.Infrastructure.Installers
{
    internal static class DatabaseInstaller
    {
        internal static IServiceCollection SetupDatabaseAccess(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IDataPackDbContext, DataPackDbContext>(options =>
            {
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                // ServiceLifetime transient solves most of concurrency issues for DbContext
                //ServiceLifetime.Transient);
            });

            services.AddDbContext<IInfoPackDbContext, InfoPackDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    sqlServerOptionsAction: sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    }));

            services.AddScoped<IDataPackReadRepository, DataPackReadRepository>();

            return services;
        }
    }
}