using System;
using System.Threading.Tasks;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Utilities;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace KokeiroLifeLogger.Functions
{
    public static class NicoNicoMyListObserver
    {
        [FunctionName("NicoNicoMyListObserver")]
        public static async Task Run(
            [TimerTrigger("0 0 15 * * *")]TimerInfo myTimer, 
            ILogger logger
        )
        {
            var myListId = 63412739;
            //var storageAccount = CloudStorageAccountUtility.GetDefaultStorageAccount();

            logger.LogInformation("get connection string");
            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");

            logger.LogInformation("get connection string");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            logger.LogInformation("new service");
            var service = new NicoNicoMyListObserveService(storageAccount);

            logger.LogInformation("GetMyListItems");
            var myListItems = await service.GetMyListItems(myListId);

            logger.LogInformation("SaveMyListItems");
            await service.SaveMyListItems(myListItems);

            logger.LogInformation($"function finished: {DateTime.UtcNow}");
        }
    }
}
