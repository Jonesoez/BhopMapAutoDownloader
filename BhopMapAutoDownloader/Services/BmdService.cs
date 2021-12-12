using BhopMapAutoDownloader.Helpers;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        private static string API_URL { get; set; }
        private static readonly string BASE_URL = "https://gamebanana.com/apiv7/Mod/ByCategory?_csvProperties=_idRow,_sName,_aSubmitter,_aFiles,_aGame&_aCategoryRowIds[]=5568&_sOrderBy=_tsDateAdded,DESC&_nPerpage=";
        private static readonly HttpClientHandler handler = new HttpClientHandler();
        private static readonly HttpClient _client = new HttpClient(handler);

        private readonly DbService _dbservice;
        private readonly FileService _fileservice;
        private readonly ILogger<BmdService> _log;
        private readonly IConfiguration _config;

        public BmdService(DbService dbservice, FileService fileservice, ILogger<BmdService> log, IConfiguration config)
        {
            _dbservice = dbservice;
            _fileservice = fileservice;
            _log = log;
            _config = config;

            API_URL = BASE_URL + _config.GetValue<string>("NumberOfMapsToCheck");
            Directory.CreateDirectory(_config.GetValue<string>("DownloadPath"));
            Directory.CreateDirectory(_config.GetValue<string>("ExtractPath"));
            if (_config.GetValue<bool>("EnableFastDlCompression"))
                Directory.CreateDirectory(_config.GetValue<string>("FastDlPath"));
        }

        public async Task<string> GetRecentUploads()
        {
            try
            {
                using var response = await _client.GetAsync(API_URL);

                if(response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }

            return null;
        }

        public async Task Run()
        {
            while (true)
            {
                var _mapinfos = JsonConvert.DeserializeObject<Gamebanana.Data[]>(await GetRecentUploads().ConfigureAwait(false));

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

                        if (_config.GetSection("MapTypes").GetChildren().Any(m => items._sName.Contains(m.Value, StringComparison.OrdinalIgnoreCase)))
                        {
                            _log.LogInformation("Found new map: {mapname} by {mapsubmitter}", items._sName, items._aSubmitter._sName);
                            _log.LogInformation("Downloading...");

                            _dbservice.AddMap(_toadd);

                            using WebClient webclient = new WebClient();
                            webclient.DownloadFile(new Uri($"{items._aFiles[0]._sDownloadUrl}"), Path.Combine(_config.GetValue<string>("DownloadPath"), items._aFiles[0]._sFile));

                            _log.LogInformation($"Extracting...");
                            _fileservice.ExtractFile(items._aFiles[0]._sFile);

                            _log.LogInformation("Download and extraction of map \"{mapname}\" completed!", items._sName);

                            if (!_config.GetValue<bool>("KeepDownloadFiles"))
                                if (File.Exists(Path.Combine(_config.GetValue<string>("DownloadPath"), items._aFiles[0]._sFile)))
                                {
                                    _log.LogInformation("Deleting file {mapfilename}", items._aFiles[0]._sFile);
                                    File.Delete(Path.Combine(_config.GetValue<string>("DownloadPath"), items._aFiles[0]._sFile));
                                    _log.LogInformation("File deleted!");
                                }

                            if (_config.GetValue<bool>("EnableFastDlCompression"))
                            {
                                _log.LogInformation("Compressing map to FastDl folder {mapfilename}...", _fileservice.ExtractedFileName);
                                _fileservice.CompressToFastdl(_fileservice.ExtractedFileName);
                            }
                        }
                    }
                }

                _log.LogInformation("Looking for new maps...");

                await Task.Delay(TimeSpan.FromSeconds(_config.GetValue<int>("CheckInterval")), CancellationToken.None);
            }
        }
    }
}
