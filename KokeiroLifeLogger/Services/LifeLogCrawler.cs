using KokeiroLifeLogger.Functions;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface ILifeLogCrawler
    {
        Task<LifeLog> CrawlAsync();
    }

    public class LifeLogCrawler : ILifeLogCrawler
    {
        private readonly IBlogPvStringLoader blogPvStringLoader;

        public LifeLogCrawler(
            IBlogPvStringLoader blogPvStringLoader
        )
        {
            this.blogPvStringLoader = blogPvStringLoader;
        }

        private string GetTitle()
        {
            return DateTime.UtcNow.Date.ToString("yyyyMMdd") + "のライフログ";
        }

        private async Task<string> GetBodyAsync()
        {
            var now = DateTime.UtcNow;

            var from = now.AddDays(-1);
            var to = now;

            var sb = new StringBuilder();

            sb.AppendLine("※自動ポストでぇ～す※");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(await IFTTTHttpTrigger.GetDataAsync(from, to));
            sb.AppendLine(await WeightMeasurementTrigger.GetDataAsync(from, to));

            var pvInfo = await blogPvStringLoader.LoadAsync();
            sb.AppendLine("-------------------------------------"); // TODO: このhrもUtilityから取得するようにする
            sb.AppendLine($"はてなブログのPV数：{pvInfo.Hatena}");
            sb.AppendLine($"QiitaのPV数：{pvInfo.Qiita}");
            sb.AppendLine();

            try
            {
                var github = await new GitHubContributionsReader().GetContributionsAsync(to, "kokeiro001");
                sb.Append(github);
            }
            catch (Exception)
            {
                //logger.LogException(ex);
                sb.AppendLine($"GitHubのコントリビューション数：(取得失敗)");
            }

            return sb.ToString();
        }

        public async Task<LifeLog> CrawlAsync()
        {
            return new LifeLog()
            {
                Title = GetTitle(),
                Body = await GetBodyAsync()
            };
        }
    }

    public class LifeLog
    {
        public string Title;
        public string Body;
    }
}
