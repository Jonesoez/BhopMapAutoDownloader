using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Models
{
    public class Maps
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Creator { get; set; }
        public string Tier { get; set; }
        public string GameType { get; set; }
        public DateTime UploadDate { get; set; }
        public string FileName { get; set; }
        public string DownloadLink { get; set; }
    }
}
