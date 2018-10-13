using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KokeiroLifeLogger.Test
{
    public class WithingsSleepTest
    {
        [Fact]
        public async Task TestMethod1()
        {
            var withingsSleepService = GetWithingsSleepService();

            var now = DateTime.UtcNow;
            var entity = new WithingsSleepEntity("WithingsSleepEntity", now.Ticks.ToString())
            {
                Action = "test action",
                Date = now,
                InsertedTime = now,
            };

            await withingsSleepService.AddAsync(entity);
        }

        private WithingsSleepService GetWithingsSleepService()
        {
            var config = InitConfiguration();
            var connectionString = config["AzureWebJobsStorage"];
            var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
            var cloudStorageAccountProvider = new CloudStorageAccountProvider(cloudStorageAccount);
            var withingsSleepRepository = new WithingsSleepRepository(cloudStorageAccountProvider);

            return new WithingsSleepService(withingsSleepRepository);
        }

        private IConfiguration InitConfiguration()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .Build();
            return config;
        }
    }
}
