using Microsoft.Azure;
using System;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IBlogPvStringLoader
    {
        Task<BlogPvInfo> LoadAsync();
    }

    public class BlogPvStringLoader : IBlogPvStringLoader
    {
        private readonly string hatenaViewId;
        private readonly string qiitaViewId;
        private readonly IGoogleAnalyticsReader googleAnalyticsReader;

        public BlogPvStringLoader(
            string hatenaViewId, 
            string qiitaViewId,
            IGoogleAnalyticsReader googleAnalyticsReader
        )
        {
            this.hatenaViewId = hatenaViewId;
            this.qiitaViewId = qiitaViewId;
            this.googleAnalyticsReader = googleAnalyticsReader;
        }

        public async Task<BlogPvInfo> LoadAsync()
        {
            var date = DateTime.UtcNow.AddHours(9).AddDays(-1);

            var result = new BlogPvInfo();

            // はてブは１日前のデータはまだ取得できないので、更に前日のデータを取得する
            result.Hatena = await googleAnalyticsReader.ReadPv(hatenaViewId, date.AddDays(-1));

            result.Qiita = await googleAnalyticsReader.ReadPv(qiitaViewId, date);

            return result;
        }
    }

    public class BlogPvInfo
    {
        public int Hatena { get; set; }
        public int Qiita { get; set; }
    }
}
