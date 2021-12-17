using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace BhopMapAutoDownloader
{
    public class Program
    {
        public static void Main()
        {
            using var db = new BmdDatabase();
            db.Database.EnsureCreated();

            var buildsettings = new ConfigurationBuilder();
            BuildConfig(buildsettings);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(buildsettings.Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine("Logs", "Log-.txt"),
                    retainedFileCountLimit: 20,
                    rollingInterval: RollingInterval.Day,
                    rollOnFileSizeLimit: true)
                .CreateLogger();

            Log.Information("BMD Starting Up...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
               {
                   services.AddDbContext<BmdDatabase>();
                   services.AddHttpClient();
                   services.AddSingleton<DbService>();
                   services.AddSingleton<FileService>();
                   services.AddSingleton<IBmdService, BmdService>();
                   services.AddHostedService<ConsumeBmdService>();
               })
                .UseSerilog()
                .UseConsoleLifetime()
                .Build();

            using (host)
            {
                host.Run();
            }
        }

        public static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}