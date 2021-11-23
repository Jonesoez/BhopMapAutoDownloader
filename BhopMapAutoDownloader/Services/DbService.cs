using BhopMapAutoDownloader.Helpers;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using BhopMapAutoDownloader.Services;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    public class DbService
    {
        private readonly BmadDatabase _db;

        public DbService(BmadDatabase db)
        {
            _db = db;
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
                LoggerService.Log(e.Message, LoggerService.LogType.ERROR);
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
