using BhopMapAutoDownloader.Infrastructure;
using BhopMapAutoDownloader.Models;
using BhopMapAutoDownloader.Services;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace BMADUnitTests
{
    public class UnitTests
    {

        [Fact]
        public void CreateDatabaseSuccessTest()
        {
            var context = new BmdDatabase();
            Assert.True(context.Database.EnsureCreated());
        }

        [Fact]
        public void DeleteDatabaseSuccessTest()
        {
            var context = new BmdDatabase();
            Assert.True(context.Database.EnsureDeleted());
        }

        [Fact]
        public void AddMapToDatabase()
        {
            var context = new BmdDatabase();
            var bot_service = new DbService(context);

            Maps _toadd = new()
            {
                Name = "Test",
                Tier = "Test Tier 1337",
                GameType = "Test CSS",
                FileName = "bhop_test",
                DownloadLink = "https://test.download/dl/bhop_test",
                Creator = "Mr. J0N3S",
                UploadDate = DateTime.Now
            };

            bot_service.AddMap(_toadd);

            Assert.True(bot_service.GetMap("Test").Name == "Test");
        }
    }
}
