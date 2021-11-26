using BhopMapAutoDownloader.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace BhopMapAutoDownloader.Infrastructure
{
    public class Settings
    {
        public string DownloadPath { get; set; } = Path.Combine("tmp_maps");
        public bool KeepDownloadFiles { get; set; } = false;
        public string ExtractPath { get; set; } = Path.Combine("extracted_maps");
        public bool EnableFastDlCompression { get; set; } = false;
        public string FastDlPath { get; set; } = @"\var\lib\pterodactyl\volumes\fastdl\";
        public List<string> MapTypes { get; set; } = new List<string> { "bhop", "kz", "autobhop" };
        public int CheckInterval { get; set; } = 300;
        public int NumberOfMapsToCheck { get; set; } = 3;

        public void LoadSettings()
        {
            string _sfilename = "settings.json";

            if (!File.Exists(_sfilename))
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new Settings { }, Formatting.Indented));

            using StreamReader _settingsfile = new StreamReader(_sfilename.ToLower());

            if (_settingsfile == null)
                return;

            try
            {
                var _settings = JsonConvert.DeserializeObject<Settings>(_settingsfile.ReadToEnd());

                DownloadPath = _settings.DownloadPath;
                ExtractPath = _settings.ExtractPath;
                KeepDownloadFiles = _settings.KeepDownloadFiles;
                EnableFastDlCompression = _settings.EnableFastDlCompression;
                FastDlPath = _settings.FastDlPath;
                MapTypes = _settings.MapTypes.ConvertAll(m => m.ToLower());

                if (5 <= _settings.CheckInterval && _settings.CheckInterval <= 86400) //allow up to every 24 hours checks/api calls
                    CheckInterval = _settings.CheckInterval;

                if (1 <= _settings.NumberOfMapsToCheck && _settings.NumberOfMapsToCheck <= 50) // 50 is max on gb api
                    NumberOfMapsToCheck = _settings.NumberOfMapsToCheck;
            }
            catch (Exception e)
            {
                LoggerService.Log(e.Message);
            }
        }
    }
}
