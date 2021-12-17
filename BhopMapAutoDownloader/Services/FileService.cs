using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace BhopMapAutoDownloader.Services
{
    public class FileService
    {
        public string ExtractedFileName { get; set; }

        private readonly IConfiguration _config;
        private readonly ILogger<FileService> _log;
        private readonly IHttpClientFactory _httpclientfactory;

        public FileService(IConfiguration configs, ILogger<FileService> log, IHttpClientFactory httpclientfactory)
        {
            _httpclientfactory = httpclientfactory;
            _config = configs;
            _log = log;
        }

        public void DownloadFile(string url, string filename)
        {
            using var _client = _httpclientfactory.CreateClient();
            using var response = _client.GetAsync(url);
            using (var stream = response.Result.Content.ReadAsStreamAsync())
            {
                var fileInfo = new FileInfo(Path.Combine(_config.GetValue<string>("DownloadPath"), filename));
                using var fileStream = fileInfo.OpenWrite();
                stream.Result.CopyTo(fileStream);
                stream.Result.Close();
            }
        }
        public void ExtractFile(string compressedFile)
        {
            if (!File.Exists(Path.Combine(_config.GetValue<string>("DownloadPath"), compressedFile)))
                return;

            try 
            {
                using Stream stream = File.OpenRead(Path.Combine(_config.GetValue<string>("DownloadPath"), compressedFile));
                using var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(_config.GetValue<string>("ExtractPath"), new ExtractionOptions()
                        {
                            ExtractFullPath = true,
                            Overwrite = true
                        });

                        if (Path.GetExtension(reader.Entry.Key) == ".bsp")
                        {
                            ExtractedFileName = reader.Entry.Key;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }
        }

        public void CompressToFastdl(string filename)
        {
            var _mapbsp = Path.Combine(_config.GetValue<string>("ExtractPath"), filename);
            var _tobz2 = Path.Combine(_config.GetValue<string>("FastDlPath"), filename + ".bz2");

            if (!File.Exists(_mapbsp))
                return;

            if (File.Exists(_tobz2))
            {
                _log.LogInformation("Map already compressed and existing in {FastDlFolder}", _config.GetValue<string>("FastDlPath"));
                return;
            }

            try
            {
                BZip2.Compress(File.OpenRead(_mapbsp), File.Create(_tobz2), true, 9);
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }
        }
    }
}
