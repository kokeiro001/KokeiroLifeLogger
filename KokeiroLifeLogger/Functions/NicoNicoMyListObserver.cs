using System;
using System.Threading.Tasks;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Utilities;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;

namespace KokeiroLifeLogger.Functions
{
    public static class NicoNicoMyListObserver
    {
        [FunctionName("NicoNicoMyListObserver")]
        public static async Task Run([TimerTrigger("0 0 15 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            var myListId = 63412739;
            log.Info($"C# Timer trigger function executed at: {DateTime.UtcNow}");

            //var storageAccount = CloudStorageAccountUtility.GetDefaultStorageAccount();

            log.Info("get connection string");
            var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");

            log.Info("get connection string");
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            log.Info("new service");
            var service = new NicoNicoMyListObserveService(storageAccount);

            log.Info("GetMyListItems");
            var myListItems = await service.GetMyListItems(myListId);

            log.Info("SaveMyListItems");
            await service.SaveMyListItems(myListItems);

            log.Info($"function finished: {DateTime.UtcNow}");
        }
    }
}
