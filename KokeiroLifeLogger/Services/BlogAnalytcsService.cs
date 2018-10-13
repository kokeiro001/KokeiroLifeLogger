using System;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IBlogAnalytcsService
    {
        Task<BlogPvInfo> LoadPvInfoAsync();
    }

    public class BlogAnalytcsService : IBlogAnalytcsService
    {
        private readonly string hatenaViewId;
        private readonly string qiitaViewId;
        private readonly IGoogleAnalyticsService googleAnalyticsReader;

        public BlogAnalytcsService(
            string hatenaViewId, 
            string qiitaViewId,
            IGoogleAnalyticsService googleAnalyticsReader
        )
        {
            this.hatenaViewId = hatenaViewId;
            this.qiitaViewId = qiitaViewId;
            this.googleAnalyticsReader = googleAnalyticsReader;
        }

        public async Task<BlogPvInfo> LoadPvInfoAsync()
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
