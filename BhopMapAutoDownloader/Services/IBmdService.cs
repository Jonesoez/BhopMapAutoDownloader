using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    internal interface IBmdService
    {
        Task CheckForNewMaps(CancellationToken stoppingToken);
    }
}
