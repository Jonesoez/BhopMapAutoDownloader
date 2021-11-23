using BhopMapAutoDownloader.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BhopMapAutoDownloader.Infrastructure
{
    public class Settings
    {
        public string DownloadPath { get; set; }
        public bool KeepDownloadFiles { get; set; }
        public string ExtractPath { get; set; }
        public string FastDlPath { get; set; }
        public int CheckInterval { get; set; }

        public void LoadSettings()
        {
            string _sfilename = "settings.json";

            if (!File.Exists(_sfilename))
                File.WriteAllText("settings.json", JsonConvert.SerializeObject(new Settings { }, Formatting.None));

            using StreamReader _settingsfile = new StreamReader(_sfilename);

            if (_settingsfile == null)
                return;

            try
            {
                var _settings = JsonConvert.DeserializeObject<Settings>(_settingsfile.ReadToEnd());

                DownloadPath = string.IsNullOrEmpty(_settings.DownloadPath) ? "tmp_maps/" : _settings.DownloadPath;

                ExtractPath = string.IsNullOrEmpty(_settings.ExtractPath) ? "extracted_maps/" : _settings.ExtractPath;

                KeepDownloadFiles = string.IsNullOrEmpty(_settings.KeepDownloadFiles.ToString()) ? true : false;

                FastDlPath = string.IsNullOrEmpty(_settings.FastDlPath) ? "/var/lib/pterodactyl/volumes/fastdl/" : _settings.FastDlPath;

                if (5 <= _settings.CheckInterval && _settings.CheckInterval <= 86400) //allow up to every 24 hours checks/api calls
                    CheckInterval = _settings.CheckInterval;
                else
                    CheckInterval = 300;
            }
            catch (Exception e)
            {
                LoggerService.Log(e.Message);
            }
        }
    }
}
