using System;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

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

            var myListItems = await nicoNicoMyListObserveService.GetMyListItems(myListId);

            await nicoNicoMyListObserveService.AddAsync(myListItems);
        }
    }
}
