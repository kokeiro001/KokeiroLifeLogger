using Microsoft.Azure.WebJobs;
using System.Configuration;
using Microsoft.Azure;
using System.Threading.Tasks;
using KokeiroLifeLogger.Services;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;

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
            [Inject]ILifeLogCrawler lifeLogCrawler,
            [Inject]IMailSender mailSender
        )
        {
            // TODO: これもBinderに移したい
            var from = ConfigurationManager.AppSettings["MixiPostMail"];
            var to = ConfigurationManager.AppSettings["MixiPostMailTo"];

            var lifeLog = await lifeLogCrawler.CrawlAsync();

            var isLocal = CloudConfigurationManager.GetSetting("IsLocal");
            if (isLocal == "true")
            {
                logger.LogInformation($"local running!!! skip SendMail. Title={lifeLog.Title}, Body={lifeLog.Body}");
                return;
            }

            mailSender.Send(to, lifeLog.Title, lifeLog.Body);
        }
    }
}