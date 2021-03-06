using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Postident.Application;
using Postident.Infrastructure;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace PostidentCheck
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            // Default logger
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .CreateLogger();

            // Dependency injection configuration
            var host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration(BuildConfig)
                .ConfigureServices((context, services) =>
                {
                    services
                        .AddTransient<MainApp>()
                        .AddOptions()
                        .AddApplicationServices()
                        .AddInfrastructureServices(context.Configuration);
                })
                .UseSerilog()
                .Build();

            // Actual main application start
            try
            {
                var mainAppInstance = ActivatorUtilities.CreateInstance<MainApp>(host.Services);
                await mainAppInstance.Run(args);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application crashed! Logging unhandled exceptions now.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            var pcWideAppsettingsLocation = $"{Environment.GetEnvironmentVariable("APPSETT_FILE_LOCATION")}\\";
            var currentEnvAppSettingsFileName = $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIROMENT") ?? "Production"}.json";

            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile(pcWideAppsettingsLocation + currentEnvAppSettingsFileName, optional: true)
                .AddJsonFile(currentEnvAppSettingsFileName, optional: true)
                .AddJsonFile("Common/DefaultShipmentValues.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Common/DefaultNamingMap.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets(typeof(Program).Assembly, optional: true);
        }
    }
}