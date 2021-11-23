using BhopMapAutoDownloader.Helpers;
using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using Newtonsoft.Json;
using SevenZipExtractor;
using SharpCompress.Common;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Readers;
using SharpCompress.Writers;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Services
{
    public class BmadService
    {
        private readonly WebClient webclient = new WebClient();
        private static readonly HttpClientHandler _handler = new HttpClientHandler();
        private static readonly HttpClient _client = new HttpClient(_handler);
        private static readonly string API_URL = "https://gamebanana.com/apiv7/Mod/ByCategory?_csvProperties=_idRow,_sName,_aSubmitter,_aFiles,_aGame&_aCategoryRowIds[]=5568&_sOrderBy=_tsDateAdded,DESC&_nPerpage=3";

        private readonly DbService _dbservice;
        private readonly Settings _settings;

        public BmadService(DbService dbservice, Settings settings)
        {
            _dbservice = dbservice;
            _settings = settings;

            if (!Directory.Exists(_settings.DownloadPath))
                Directory.CreateDirectory(_settings.DownloadPath);
            if (!Directory.Exists(_settings.ExtractPath))
                Directory.CreateDirectory(_settings.ExtractPath);
        }

        public static async Task<string> GetRecentUploads()
        {
            using var result = await _client.GetAsync(API_URL);
            return await result.Content.ReadAsStringAsync();
        }

        public string ExtractedFile(string compressedFile)
        {
            if (!File.Exists(_settings.DownloadPath + compressedFile))
                return null;

            using ArchiveFile archiveFile = new ArchiveFile(_settings.DownloadPath + compressedFile);
            archiveFile.Extract(_settings.ExtractPath, true);

            return archiveFile.Entries.FirstOrDefault().FileName;
        }

        public void CompressToFastdl(string filename)
        {
            if (!File.Exists(_settings.ExtractPath + filename))
                return;

            //TODO
        }

        public async Task CheckForNewMaps()
        {
            while (true)
            {
                var _infos = JsonConvert.DeserializeObject<Gamebanana.Data[]>(await GetRecentUploads());

                foreach (var items in _infos)
                {
                    var _map = _dbservice.GetMap(items._sName);
                    if (_map == null)
                    {
                        Maps _toadd = new Maps()
                        {
                            Name = items._sName,
                            Creator = items._aSubmitter._sName,
                            Tier = "undefined",
                            UploadDate = TimeStamp.UnixTimeStampToDateTime(items._aFiles[0]._tsDateAdded),
                            DownloadLink = items._aFiles[0]._sDownloadUrl
                        };

                        LoggerService.Log($"Found new map: {items._sName} by {items._aSubmitter._sName}");
                        LoggerService.Log($"Downloading...");

                        _dbservice.AddMap(_toadd);

                        webclient.DownloadFile(new Uri($"{items._aFiles[0]._sDownloadUrl}"), _settings.DownloadPath + items._aFiles[0]._sFile);

                        LoggerService.Log($"Extracting...");

                        var _tocompress = ExtractedFile(items._aFiles[0]._sFile);
                        
                        LoggerService.Log($"Download and extraction of map \"{items._sName}\" completed!", LoggerService.LogType.DONE);
                        LoggerService.Log($"Deleting file {items._aFiles[0]._sFile}", LoggerService.LogType.INFO);

                        if(!_settings.KeepDownloadFiles)
                            if(File.Exists(_settings.DownloadPath + items._aFiles[0]._sFile))
                                File.Delete(_settings.DownloadPath + items._aFiles[0]._sFile);

                        LoggerService.Log($"Compressing to bz2 for FastDl {items._aFiles[0]._sFile}", LoggerService.LogType.INFO);

                        CompressToFastdl(_tocompress);

                        LoggerService.Log($"File deleted!\n", LoggerService.LogType.DONE);
                    }
                }

                LoggerService.Log($"Looking for new maps...", LoggerService.LogType.INFO);

                await Task.Delay(TimeSpan.FromSeconds(_settings.CheckInterval), CancellationToken.None);
            }
        }
    }
}
