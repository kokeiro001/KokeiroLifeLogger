using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Utilities;
using Microsoft.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class NicoNicoMyListObserver
    {
        [FunctionName("NicoNicoMyListObserver")]
        public static async Task Run(
            [TimerTrigger("0 0 15 * * *")]TimerInfo myTimer, 
            ILogger logger,
            [Inject]INicoNicoMyListObserveService nicoNicoMyListObserveService
        )
        {
            var myListId = 63412739;

            logger.LogInformation("new service");

            logger.LogInformation("GetMyListItems");
            var myListItems = await nicoNicoMyListObserveService.GetMyListItems(myListId);

            logger.LogInformation("SaveMyListItems");
            await nicoNicoMyListObserveService.AddAsync(myListItems);

            logger.LogInformation($"function finished: {DateTime.UtcNow}");
        }
    }
}
