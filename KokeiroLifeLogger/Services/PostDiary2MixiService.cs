using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IPostDiary2MixiService
    {
        Task PostDiary();
    }

    public class PostDiary2MixiService : IPostDiary2MixiService
    {
        private readonly ILifeLogService lifeLogCrawler;
        private readonly IMailService mailSender;
        private readonly string sendTo;

        public PostDiary2MixiService(
            ILifeLogService lifeLogCrawler,
            IMailService mailSender,
            string sendTo
        )
        {
            this.lifeLogCrawler = lifeLogCrawler;
            this.mailSender = mailSender;
            this.sendTo = sendTo;
        }

        public async Task PostDiary()
        {
            var lifeLog = await lifeLogCrawler.CrawlAsync();

            mailSender.Send(sendTo, lifeLog.Title, lifeLog.Body);
        }
    }
}
