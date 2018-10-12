using System;
using System.Threading.Tasks;
using KokeiroLifeLogger.Injection;
using KokeiroLifeLogger.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace KokeiroLifeLogger.Functions
{
    public static class NicoNicoMyListObserver
    {
        [FunctionName("NicoNicoMyListObserver")]
        public static async Task Run(
            [TimerTrigger("0 0 15 * * *")]TimerInfo myTimer, 
            ILogger logger,
            [Inject(typeof(INicoNicoMyListObserveService))]INicoNicoMyListObserveService nicoNicoMyListObserveService
        )
        {
            var myListId = 63412739;

            var myListItems = await nicoNicoMyListObserveService.GetMyListItems(myListId);

            await nicoNicoMyListObserveService.AddAsync(myListItems);
        }
    }
}
