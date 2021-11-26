using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using BhopMapAutoDownloader.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BhopMapAutoDownloader
{
    public class Program
    {
        public static void Main()
        {
            using var db = new BmdDatabase();
            db.Database.EnsureCreated();

            Settings _settings = new Settings();
            FileService _fileservice = new FileService(_settings);
            _settings.LoadSettings();

            DbService _botservice = new DbService(db);
            BmdService _bmadservice = new BmdService(_botservice, _settings, _fileservice);

            _bmadservice.CheckForNewMaps().GetAwaiter().GetResult();
        }
    }
}