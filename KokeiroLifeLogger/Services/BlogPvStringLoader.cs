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

        public BlogPvStringLoader(string hatenaViewId, string qiitaViewId)
        {
            this.hatenaViewId = hatenaViewId;
            this.qiitaViewId = qiitaViewId;
        }

        public async Task<BlogPvInfo> LoadAsync()
        {
            var date = DateTime.UtcNow.AddHours(9).AddDays(-1);

            var pvReader = new GoogleAnalyticsReader();

            var result = new BlogPvInfo();

            // はてブは１日前のデータはまだ取得できないので、更に前日のデータを取得する
            result.Hatena = await pvReader.ReadPv(hatenaViewId, date.AddDays(-1));

            result.Qiita = await pvReader.ReadPv(qiitaViewId, date);

            return result;
        }
    }

    public class BlogPvInfo
    {
        public int Hatena { get; set; }
        public int Qiita { get; set; }
    }
}
