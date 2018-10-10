﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Parser.Html;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;

namespace KokeiroLifeLogger.Services
{
    public interface IGitHubContributionsReader
    {
        Task<string> GetContributionsAsync(DateTime date, string username);
    }

    class GitHubContributionsReader : IGitHubContributionsReader
    {
        static readonly string HostUrl = @"https://github.com/";
        static HttpClient httpClient = new HttpClient();

        public GitHubContributionsReader()
        {
        }

        public async Task<string> GetContributionsAsync(DateTime date, string username)
        {
            var sb = new StringBuilder();
            sb.AppendLine("-------------------------------------");

            var contoributions = await GetContributionsCoreAsync(username);
            var contribution = contoributions.First(x => x.Date == date.Date);

            sb.AppendLine($"GitHubのコントリビューション数：{contribution.Contributions}");
            sb.AppendLine($"TargetDate：{date}");
            sb.AppendLine();

            return sb.ToString();
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

    class ContributionItem
    {
        public DateTime Date { get; set; }
        public int Contributions { get; set; }
    }
}
