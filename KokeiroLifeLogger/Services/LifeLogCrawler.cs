using AzureFunctions.Autofac;
using KokeiroLifeLogger.Functions;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IIFTTTService iftttService;
        private readonly IWeightMeasurementService weightMeasurementService;

        public LifeLogCrawler(
            IBlogPvStringLoader blogPvStringLoader,
            IIFTTTService iftttService,
            IWeightMeasurementService weightMeasurementService
        )
        {
            this.blogPvStringLoader = blogPvStringLoader;
            this.iftttService = iftttService;
            this.weightMeasurementService = weightMeasurementService;
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

            try
            {
                var iftttData = iftttService.GetDataByDate(from, to);
                AppendStringByPartitionKey(iftttData, sb, "pocket", "Pocketに突っ込んだ記事");
                AppendStringTwitterLiked(iftttData, sb);
                AppendStringByPartitionKey(iftttData, sb, "github_star", "GitHubでStarつけたリポジトリ");
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }


            sb.AppendLine("-------------------------------------");

            try
            {
                var weightMeasurement = weightMeasurementService.GetByDate(from, to);
                if (weightMeasurement != null)
                {
                    sb.AppendLine($"体重:{weightMeasurement.Weight}kg");
                    sb.AppendLine($"除脂肪体重:{weightMeasurement.LeanMass}kg");
                    sb.AppendLine($"体脂肪量:{weightMeasurement.FatMass}kg");
                    sb.AppendLine($"体脂肪率:{weightMeasurement.FatPercent}%");
                }
                else
                {
                    sb.AppendLine($"(今日は体重計乗ってないらしい)");
                }
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }
            sb.AppendLine();

            sb.AppendLine("-------------------------------------");

            try
            {
                var pvInfo = await blogPvStringLoader.LoadAsync();
                sb.AppendLine($"はてなブログのPV数：{pvInfo.Hatena}");
                sb.AppendLine($"QiitaのPV数：{pvInfo.Qiita}");
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.AppendLine();
            sb.AppendLine("-------------------------------------");

            try
            {
                var github = await new GitHubContributionsReader().GetContributionsAsync(to, "kokeiro001");
                sb.Append(github);
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            return sb.ToString();
        }
        private void AppendStringByPartitionKey(IEnumerable<IFTTTEntity> items, StringBuilder sb, string key, string title)
        {
            sb.AppendLine("-------------------------------------");
            sb.AppendLine(title);
            sb.AppendLine();
            var targetItems = items.Where(x => x.PartitionKey == key).ToArray();

            if (targetItems.Length == 0)
            {
                sb.AppendLine("(なし)");
            }
            else
            {
                foreach (var item in targetItems)
                {
                    sb.AppendLine(item.Title);
                    sb.AppendLine(item.Url);
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
        }
        private void AppendStringTwitterLiked(IEnumerable<IFTTTEntity> items, StringBuilder sb)
        {
            sb.AppendLine("-------------------------------------");
            sb.AppendLine("Twitterでイイねしたツイート");
            sb.AppendLine();
            var targetItems = items.Where(x => x.PartitionKey == "twitter_like").ToArray();
            if (targetItems.Length == 0)
            {
                sb.AppendLine("(なし)");
            }
            else
            {
                foreach (var item in targetItems)
                {
                    // httpとか含むなら、そいつは削除する
                    var title = StringUtility.RemoveHttp(item.Title).TrimEnd();
                    if (!string.IsNullOrEmpty(title))
                    {
                        sb.AppendLine(title);
                    }
                    sb.AppendLine(item.Url);
                    sb.AppendLine();
                }
            }
            sb.AppendLine();
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
