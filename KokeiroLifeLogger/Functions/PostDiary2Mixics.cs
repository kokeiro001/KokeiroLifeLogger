using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Functions
{
    // 1日の区切りをAM5:00とした日記を自動投稿する.
    // 時差注意！

    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class PostDiary2Mixics
    {
        [FunctionName("PostDiary2Mixics")]
        public static async Task Run(
            [TimerTrigger("0 0 20 * * *")]TimerInfo myTimer, 
            ILogger logger,
            [Inject]IPostDiary2MixiService postDiary2MixiService,
            [Inject]IConfigProvider configProvider
        )
        {
            var config = configProvider.GetConfig();

            if (config["IsLocal"] == "true")
            {
                return;
            }
            await postDiary2MixiService.PostDiary();
        }
    }
}