using BhopMapAutoDownloader.Infrastructure;
using SevenZipExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BhopMapAutoDownloader.Services
{
    public class FileService
    {
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
            archiveFile.Extract(_settings.ExtractPath, true);
            //return archiveFile.Entries.FirstOrDefault().FileName;
        }

        public void CompressToFastdl(string filename)
        {
            if (!File.Exists(Path.Combine(_settings.ExtractPath, filename)))
                return;

            //TODO
        }
    }
}
