using BhopMapAutoDownloader.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BhopMapAutoDownloader.Infrastructure
{
    public class BmadDatabase : DbContext
    {
        public DbSet<Maps> Maps => Set<Maps>();
        public BmadDatabase() : base() { }
        public BmadDatabase(DbContextOptions options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=BMAD_Database.db");
                optionsBuilder.EnableSensitiveDataLogging(true);
            }
        }
    }
}
