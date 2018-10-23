using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;

namespace KokeiroLifeLogger.Functions.Test
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class PostDiary2MixiTest
    {
        [FunctionName("PostDiary2MixiTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger logger,
            [Inject]ILifeLogService lifeLogCrawler
        )
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var lifeLog = await lifeLogCrawler.CrawlAsync();

            logger.LogInformation($"Title={lifeLog.Title}, Body={lifeLog.Body}");

            return new OkObjectResult(lifeLog.Body);
        }
    }
}
