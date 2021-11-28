using BhopMapAutoDownloader.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SevenZipExtractor;
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

            using ArchiveFile archiveFile = new ArchiveFile(Path.Combine(_config.GetValue<string>("DownloadPath"), compressedFile));

            try
            {
                var _bspfile = archiveFile.Entries.Where(m => Path.GetExtension(m.FileName) == ".bsp").FirstOrDefault();

                archiveFile.Extract(_config.GetValue<string>("ExtractPath"), true);
                ExtractedFileName = _bspfile.FileName;
            }
            catch (Exception e)
            {
                _log.LogError(e.Message);
            }
        }

        public void CompressToFastdl(string filename)
        {
            if (!File.Exists(Path.Combine(_config.GetValue<string>("ExtractPath"), filename)))
                return;

            //TODO
        }
    }
}
