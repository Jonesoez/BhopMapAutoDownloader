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
            using var db = new BmadDatabase();
            db.Database.EnsureCreated();

            Settings _settings = new Settings();
            _settings.LoadSettings();

            DbService _botservice = new DbService(db);
            BmadService _bmadservice = new BmadService(_botservice, _settings);

            _bmadservice.CheckForNewMaps().GetAwaiter().GetResult();
        }
    }
}