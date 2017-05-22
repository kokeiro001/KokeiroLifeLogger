using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger
{
    class LifeLogCrawler
    {
        private static string GetTitle()
        {
            return DateTime.Now.Date.ToString("yyyymmdd") + "のライフログ";
        }

        private static async Task<string> GetBodyAsync()
        {
            var now = DateTime.Now;

            var from = now.Date.AddDays(-1);
            var to = now;

            var sb = new StringBuilder();

            sb.AppendLine("※自動ポスト※");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(await IFTTTHttpTrigger.GetData(from, to));

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
