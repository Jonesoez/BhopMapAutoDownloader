using BhopMapAutoDownloader.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Infrastructure
{
    public class BmdDatabase : DbContext
    {
        public DbSet<Maps> Maps => Set<Maps>();
        public BmdDatabase() : base() { }
        public BmdDatabase(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=BMD_Database.db");
                optionsBuilder.EnableSensitiveDataLogging(true);
            }
        }
    }
}
