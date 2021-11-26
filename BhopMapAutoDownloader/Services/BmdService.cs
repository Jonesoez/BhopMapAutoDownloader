using BhopMapAutoDownloader.Helpers;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    public class BmdService
    {
        private static readonly string API_URL = $"https://gamebanana.com/apiv7/Mod/ByCategory?_csvProperties=_idRow,_sName,_aSubmitter,_aFiles,_aGame&_aCategoryRowIds[]=5568&_sOrderBy=_tsDateAdded,DESC&_nPerpage=3";

        private readonly DbService _dbservice;
        private readonly Settings _settings;
        private readonly FileService _fileservice;

        public BmdService(DbService dbservice, Settings settings, FileService fileservice)
        {
            _dbservice = dbservice;
            _settings = settings;
            _fileservice = fileservice;

            Directory.CreateDirectory(_settings.DownloadPath);
            Directory.CreateDirectory(_settings.ExtractPath);
        }

        public static async Task<string> GetRecentUploads()
        {
            using HttpClient _client = new HttpClient();
            _client.BaseAddress = new Uri(API_URL);
            HttpResponseMessage response = await _client.GetAsync(API_URL);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();
            
            return null;
        }

        public async Task CheckForNewMaps()
        {
            while (true)
            {
                var _mapinfos = JsonConvert.DeserializeObject<Gamebanana.Data[]>(await GetRecentUploads());

                foreach (var items in _mapinfos)
                {
                    if (_dbservice.GetMap(items._sName) == null)
                    {
                        Maps _toadd = new Maps()
                        {
                            Name = items._sName,
                            Creator = items._aSubmitter._sName,
                            Tier = "undefined",
                            UploadDate = TimeStamp.UnixTimeStampToDateTime(items._aFiles[0]._tsDateAdded),
                            DownloadLink = items._aFiles[0]._sDownloadUrl
                        };

                        if (_settings.MapTypes.Any(m => items._sName.Contains(m, StringComparison.OrdinalIgnoreCase)))
                        {
                            LoggerService.Log($"Found new map: {items._sName} by {items._aSubmitter._sName}");
                            LoggerService.Log($"Downloading...");

                            _dbservice.AddMap(_toadd);

                            using WebClient webclient = new WebClient();
                            webclient.DownloadFile(new Uri($"{items._aFiles[0]._sDownloadUrl}"), Path.Combine(_settings.DownloadPath, items._aFiles[0]._sFile));

                            LoggerService.Log($"Extracting...");
                            _fileservice.ExtractFile(items._aFiles[0]._sFile);

                            LoggerService.Log($"Download and extraction of map \"{items._sName}\" completed!", LoggerService.LogType.DONE);

                            if (!_settings.KeepDownloadFiles)
                                if (File.Exists(Path.Combine(_settings.DownloadPath, items._aFiles[0]._sFile)))
                                {
                                    LoggerService.Log($"Deleting file {items._aFiles[0]._sFile}", LoggerService.LogType.INFO);
                                    File.Delete(Path.Combine(_settings.DownloadPath, items._aFiles[0]._sFile));
                                    LoggerService.Log($"File deleted!\n", LoggerService.LogType.DONE);
                                }

                            if (_settings.EnableFastDlCompression)
                            {
                                LoggerService.Log($"Compressing to bz2 for FastDl {items._aFiles[0]._sFile}...", LoggerService.LogType.INFO);
                                _fileservice.CompressToFastdl(_fileservice.ExtractedFileName);
                            }
                        }
                    }
                }

                LoggerService.Log($"Looking for new maps...", LoggerService.LogType.INFO);

                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckInterval), CancellationToken.None);
            }
        }
    }
}
