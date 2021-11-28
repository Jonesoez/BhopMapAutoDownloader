using BhopMapAutoDownloader.Helpers;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using BhopMapAutoDownloader.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace BhopMapAutoDownloader.Services
{
    public class DbService
    {
        private readonly BmdDatabase _db;
        private readonly ILogger<DbService> _log;

        public DbService(BmdDatabase db, ILogger<DbService> log)
        {
            _db = db;
            _log = log;
        }

        public void AddMap(Maps map)
        {
            try
            {
                if (map != null && !_db.Maps.Any(m => m.Name == map.Name))
                {
                    _db.Add(map);
                    _db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }
        }

        public Maps GetMap(string mapName)
        {
            if (string.IsNullOrEmpty(mapName))
                return null;

            return _db.Maps.FirstOrDefault(m => m.Name == mapName);
        }
    }
}
