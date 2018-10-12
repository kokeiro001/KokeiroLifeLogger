using Microsoft.Azure.WebJobs;
using System.Configuration;
using Microsoft.Azure;
using System.Threading.Tasks;
using KokeiroLifeLogger.Services;
using Microsoft.Extensions.Logging;
using KokeiroLifeLogger.Injection;

namespace KokeiroLifeLogger.Functions
{
    // 1日の区切りをAM5:00とした日記を自動投稿する.
    // 時差注意！

    public static class PostDiary2Mixics
    {
        [FunctionName("PostDiary2Mixics")]
        public static async Task Run(
            [TimerTrigger("0 0 20 * * *")]TimerInfo myTimer, 
            ILogger logger,
            [Inject(typeof(ILifeLogCrawler))]ILifeLogCrawler lifeLogCrawler,
            [Inject(typeof(IMailSender))]IMailSender mailSender,
            [Inject(typeof(IConfigProvider))]IConfigProvider configProvider
        )
        {
            // TODO: これもBinderに移したい
            var config = configProvider.GetConfig();
            var from = config["MixiPostMail"];
            var to = config["MixiPostMailTo"];

            var lifeLog = await lifeLogCrawler.CrawlAsync();

            logger.LogInformation($"Title={lifeLog.Title}, Body={lifeLog.Body}");

            var isLocal = config["IsLocal"];
            if (isLocal == "true")
            {
                logger.LogInformation($"local running!!! skip SendMail.");
                return;
            }

            mailSender.Send(to, lifeLog.Title, lifeLog.Body);
        }
    }
}