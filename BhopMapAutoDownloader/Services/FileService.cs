using BhopMapAutoDownloader.Infrastructure;
using SevenZipExtractor;
using System;
using System.IO;
using System.Linq;

namespace BhopMapAutoDownloader.Services
{
    public class FileService
    {
        public string ExtractedFileName { get; set; }

        private readonly Settings _settings;

        public FileService(Settings settings)
        {
            _settings = settings;
        }

        public void ExtractFile(string compressedFile)
        {
            if (!File.Exists(Path.Combine(_settings.DownloadPath, compressedFile)))
                return;

            using ArchiveFile archiveFile = new ArchiveFile(Path.Combine(_settings.DownloadPath, compressedFile));

            try
            {
                var _bspfile = archiveFile.Entries.Where(m => Path.GetExtension(m.FileName) == ".bsp").FirstOrDefault();

                archiveFile.Extract(_settings.ExtractPath, true);
                ExtractedFileName = _bspfile.FileName;
            }
            catch (Exception e)
            {
                LoggerService.Log(e.Message, LoggerService.LogType.ERROR);
            }
        }

        public void CompressToFastdl(string filename)
        {
            if (!File.Exists(Path.Combine(_settings.ExtractPath, filename)))
                return;

            //TODO
        }
    }
}
