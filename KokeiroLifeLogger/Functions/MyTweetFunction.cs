using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class MyTweetFunction
    {
        [FunctionName("MyTweet")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "mytweet")] HttpRequest req,
            ILogger logger,
            [Inject]IMyTweetService myTweetService
        )
        {
            logger.LogInformation("C# HTTP trigger MyTweet function processed a request.");

            var json = await req.ReadAsStringAsync();
            logger.LogInformation(json);

            var request = JsonConvert.DeserializeObject<TweetRequest>(json);

            logger.LogInformation($"Text={request.Text}");

            await myTweetService.Run(request);

            return new OkObjectResult(json);
        }
    }
}
