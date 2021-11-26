using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Services;

namespace BhopMapAutoDownloader
{
    public class Program
    {
        public static void Main()
        {
            using var db = new BmdDatabase();
            db.Database.EnsureCreated();

            Settings _settings = new Settings();
            FileService _fileservice = new FileService(_settings);
            _settings.LoadSettings();

            DbService _dbservice = new DbService(db);
            BmdService _bmdservice = new BmdService(_dbservice, _settings, _fileservice);

            _bmdservice.CheckForNewMaps().GetAwaiter().GetResult();
        }
    }
}