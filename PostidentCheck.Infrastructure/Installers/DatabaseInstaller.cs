using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Postident.Application.Common.Interfaces;
using Postident.Infrastructure.Persistence;

namespace Postident.Infrastructure.Installers
{
    internal static class DatabaseInstaller
    {
        internal static IServiceCollection SetupParcelDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<IDataPackDbContext, DataPackDbContext>(options =>
                    // ServiceLifetime transient solves most of concurrency issues for DbContext
                    options.UseSqlServer(
                        connectionString,
                        sqlServerOptionsAction: sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 5,
                                maxRetryDelay: TimeSpan.FromSeconds(30),
                                errorNumbersToAdd: null);
                        }),
                ServiceLifetime.Transient);

            services.AddTransient<IDataPackRepository, DataPackRepository>();

            return services;
        }
    }
}