using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using KokeiroLifeLogger.Services;
using AzureFunctions.Autofac;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;

namespace KokeiroLifeLogger.Functions.Test
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class PostDiary2MixiTest
    {
        [FunctionName("PostDiary2MixiTest")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "PostDiary2MixiTest")] HttpRequest req,
            ILogger logger,
            [Inject]IPostDiary2MixiService postDiary2MixiService,
            [Inject]IConfigProvider configProvider
        )
        {
            var config = configProvider.GetConfig();

            if (config["IsLocal"] != "true")
            {
                return new BadRequestResult();
            }

            await postDiary2MixiService.PostDiary();

            return new OkResult();
        }
    }
}
