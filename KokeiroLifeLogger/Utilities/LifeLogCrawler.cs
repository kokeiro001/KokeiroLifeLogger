﻿using KokeiroLifeLogger.Functions;
using KokeiroLifeLogger.Utilities;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Utilities
{
    class LifeLogCrawler
    {
        private static string GetTitle()
        {
            return DateTime.UtcNow.Date.ToString("yyyyMMdd") + "のライフログ";
        }

        private static async Task<string> GetBodyAsync()
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
            sb.Append(await new BlogPvStringLoader().LoadAsync());

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

    class LifeLog
    {
        public string Title;
        public string Body;
    }
}
