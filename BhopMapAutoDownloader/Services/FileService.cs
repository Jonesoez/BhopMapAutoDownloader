using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.IO;
using System.Linq;

namespace BhopMapAutoDownloader.Services
{
    public class FileService
    {
        public string ExtractedFileName { get; set; }

        private readonly IConfiguration _config;
        private readonly ILogger<FileService> _log;

        public FileService(IConfiguration configs, ILogger<FileService> log)
        {
            _config = configs;
            _log = log;
        }

        public void ExtractFile(string compressedFile)
        {
            if (!File.Exists(Path.Combine(_config.GetValue<string>("DownloadPath"), compressedFile)))
                return;

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

                    if(Path.GetExtension(reader.Entry.Key) == ".bsp")
                    {
                        ExtractedFileName = reader.Entry.Key;
                    }
                }
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
