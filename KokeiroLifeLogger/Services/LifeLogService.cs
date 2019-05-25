﻿using KokeiroLifeLogger.Repositories;
using KokeiroLifeLogger.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface ILifeLogService
    {
        Task<LifeLog> CrawlAsync();
    }

    public class LifeLogService : ILifeLogService
    {
        private readonly ISlackManualDiaryService slackManualDiaryService;
        private readonly IBlogAnalytcsService blogPvStringLoader;
        private readonly IIFTTTService iftttService;
        private readonly IWeightMeasurementService weightMeasurementService;
        private readonly IGitHubService gitHubContributionsReader;
        private readonly IWithingsSleepService withingsSleepService;

        public LifeLogService(
            ISlackManualDiaryService slackManualDiaryService,
            IBlogAnalytcsService blogPvStringLoader,
            IIFTTTService iftttService,
            IWeightMeasurementService weightMeasurementService,
            IGitHubService gitHubContributionsReader,
            IWithingsSleepService withingsSleepService
        )
        {
            this.slackManualDiaryService = slackManualDiaryService;
            this.blogPvStringLoader = blogPvStringLoader;
            this.iftttService = iftttService;
            this.weightMeasurementService = weightMeasurementService;
            this.gitHubContributionsReader = gitHubContributionsReader;
            this.withingsSleepService = withingsSleepService;
        }

        private string GetTitle()
        {
            return DateTime.UtcNow.Date.ToString("yyyyMMdd") + "のライフログ";
        }

        private async Task<string> GetBodyAsync()
        {
            var now = DateTime.UtcNow;

            var fromDateUtc = now.AddDays(-1);
            var toDateUtc = now;
            var dateFormat = "yyyy/MM/dd HH:mm:ss";

            var sb = new StringBuilder();

            try
            {
                var manualDiary = (await slackManualDiaryService.GetManualDiary()).Trim();
                if (string.IsNullOrEmpty(manualDiary))
                {
                    sb.AppendLine("(マニュアル日記はありません)");
                }
                else
                {
                    sb.AppendLine(manualDiary);
                }
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }
            sb.AppendLine();
            sb.AppendLine("-------------------------------------");
            sb.AppendLine();

            sb.AppendLine("※自動ポストでぇ～す※");
            sb.AppendLine();
            sb.AppendLine();

            try
            {
                var iftttData = await iftttService.GetDataByDate(fromDateUtc, toDateUtc);
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
                var weightMeasurement = await weightMeasurementService.GetByDate(fromDateUtc, toDateUtc);
                if (weightMeasurement != null)
                {
                    sb.AppendLine($"体重:{weightMeasurement.Weight}kg");
                    sb.AppendLine($"除脂肪体重:{weightMeasurement.LeanMass}kg");
                    sb.AppendLine($"体脂肪量:{weightMeasurement.FatMass}kg");
                    sb.AppendLine($"体脂肪率:{weightMeasurement.FatPercent}%");

                    sb.AppendLine($"測定時間:" + weightMeasurement.MesuredAt.AddHours(9).ToString(dateFormat));
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
                var pageViewInfo = await blogPvStringLoader.LoadPvInfoAsync();
                sb.AppendLine($"はてなブログのPV数：{pageViewInfo.Hatena}");
                sb.AppendLine($"QiitaのPV数：{pageViewInfo.Qiita}");
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.AppendLine();
            sb.AppendLine("-------------------------------------");

            try
            {
                var githubContribution = await gitHubContributionsReader.GetContributionsAsync(toDateUtc, "kokeiro001");

                sb.AppendLine($"GitHubのコントリビューション数：{githubContribution.Contributions}");
                sb.AppendLine($"TargetDate：{githubContribution.Date.ToString(dateFormat)}");
            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.AppendLine();
            sb.AppendLine("-------------------------------------");

            try
            {
                var text = await withingsSleepService.GetDiaryData(fromDateUtc, toDateUtc);

                sb.AppendLine(text);

            }
            catch (Exception e)
            {
                sb.AppendLine(e.Message);
            }

            sb.AppendLine();

            sb.AppendLine($"(debug) {fromDateUtc.ToString(dateFormat)}-{toDateUtc.ToString(dateFormat)}");

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
        public string Title { get; set; }
        public string Body { get; set; }
    }
}
