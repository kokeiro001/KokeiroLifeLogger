using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;

namespace KokeiroLifeLogger.Services
{
    public interface IGitHubService
    {
        Task<ContributionItem> GetContributionsAsync(DateTime date, string username);
    }

    class GitHubService : IGitHubService
    {
        static readonly string HostUrl = @"https://github.com/";
        static HttpClient httpClient = new HttpClient();

        public GitHubService()
        {
        }

        public async Task<ContributionItem> GetContributionsAsync(DateTime date, string username)
        {
            var contoributions = await GetContributionsCoreAsync(username);
            var contribution = contoributions.First(x => x.Date == date.Date);

            return contribution;
        }

        async Task<IEnumerable<ContributionItem>> GetContributionsCoreAsync(string username)
        {
            var html = await GetHtmlAsync(username);
            return ParseHtml(html);
        }

        IEnumerable<ContributionItem> ParseHtml(string html)
        {
            var document = new HtmlParser().Parse(html);
            var graphCanvas = document.QuerySelectorAll("div").Where(x => x.ClassList.Contains("graph-canvas")).First();
            var dayNodes = graphCanvas.QuerySelectorAll("rect");

            return dayNodes.Select(x => new ContributionItem
            {
                Date = DateTime.Parse(x.GetAttribute("data-date")),
                Contributions = int.Parse(x.GetAttribute("data-count")),
            })
            .OrderBy(x => x.Date);
        }

        async Task<string> GetHtmlAsync(string username)
        {
            var url = HostUrl + username;
            return await httpClient.GetStringAsync(url);
        }
    }

    public class ContributionItem
    {
        public DateTime Date { get; set; }
        public int Contributions { get; set; }
    }
}
