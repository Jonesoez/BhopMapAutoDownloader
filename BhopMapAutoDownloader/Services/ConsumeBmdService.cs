using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    public class ConsumeBmdService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly IConfiguration _config;
        private readonly ILogger<ConsumeBmdService> _log;

        public ConsumeBmdService(IServiceProvider services, IConfiguration config, ILogger<ConsumeBmdService> logger)
        {
            _services = services;
            _config = config;
            _log = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _services.CreateScope())
                {
                    var scopedProcessingService =
                        scope.ServiceProvider
                            .GetRequiredService<IBmdService>();

                    await scopedProcessingService.CheckForNewMaps(stoppingToken);
                }

                await Task.Delay(TimeSpan.FromSeconds(_config.GetValue<int>("CheckInterval")), stoppingToken);
            }
        }
    }
}
