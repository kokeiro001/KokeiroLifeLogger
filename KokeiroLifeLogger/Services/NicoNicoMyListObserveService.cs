using AngleSharp.Parser.Html;
using KokeiroLifeLogger.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public class NicoNicoMyListObserveService
    {
        public static string TableName = @"nicoNicoMyList";

        readonly CloudStorageAccount cloudStorageAccount;

        public NicoNicoMyListObserveService(CloudStorageAccount cloudStorageAccount)
        {
            this.cloudStorageAccount = cloudStorageAccount;
        }

        public async Task<MyListItem[]> GetMyListItems(int myListId)
        {
            var url = $"http://www.nicovideo.jp/mylist/{myListId}";

            var html = "";
            using (var httpClient = new HttpClient())
            {
                var resopnse = await httpClient.GetAsync(url);
                html = await resopnse.Content.ReadAsStringAsync();
            }

            var htmlParser = new HtmlParser();
            var htmlDocument = htmlParser.Parse(html);

            var target = htmlDocument.QuerySelectorAll("script")
                .Where(x => x.GetAttribute("type") == "text/javascript")
                .Select(x => x.TextContent)
                .Where(x => x.Trim().StartsWith("<!--"))
                .Where(x => x.Contains("Jarty.globals({"))
                .Single();

            var targetLine = target.Split('\n')
                .Where(x => x.Trim().StartsWith(@"Mylist.preload("))
                .Single();

            var json = targetLine
                .Trim()
                .TrimStarts(@"Mylist.preload\(63412739, ")
                .TrimEnds(@"\);");

            var dezerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            };

            var myListItems = JsonConvert.DeserializeObject<MyListItem[]>(json, dezerializerSettings);

            return myListItems;
        }

        public async Task SaveMyListItems(IEnumerable<MyListItem> myListItems)
        {
            var now = DateTime.UtcNow;
            var nowTicks = now.Ticks;
            var entities = myListItems
                .Select(x => new NicoNicoMyListEntity("niconico_mylist", $"{x.ItemData.VideoId}_{nowTicks}")
                {
                    Title = x.ItemData.Title,
                    VideoId = x.ItemData.VideoId,
                    ViewCounter = x.ItemData.ViewCounter,
                    CommentCounter = x.ItemData.NumRes,
                    MyListCounter = x.ItemData.MylistCounter,
                    InsertedTime = now,
                });

            var table = await GetCloudTableAsync();

            foreach (var entity in entities)
            {
                var op = TableOperation.InsertOrReplace(entity);

                await table.ExecuteAsync(op);
            }
        }

        private async Task<CloudTable> GetCloudTableAsync()
        {
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }

    static class StringEx
    {
        public static string TrimStarts(this string str, string trimString)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, $"^{trimString}", "");
        }
        public static string TrimEnds(this string str, string trimString)
        {
            return System.Text.RegularExpressions.Regex.Replace(str, $"{trimString}$", "");
        }
    }


    public class MyListItem
    {
        public int ItemType { get; set; }
        public string ItemId { get; set; }
        public string Description { get; set; }
        public MovieInfo ItemData { get; set; }
        public int Watch { get; set; }
        public int CreateTime { get; set; }
        public int UpdateTime { get; set; }
    }

    public class MovieInfo
    {
        public string VideoId { get; set; }
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public int FirstRetrieve { get; set; }
        public int UpdateTime { get; set; }
        public int ViewCounter { get; set; }
        public int MylistCounter { get; set; }
        public int NumRes { get; set; }
        public string GroupType { get; set; }
        public string LengthSeconds { get; set; }
        public string Deleted { get; set; }
        public string LastResBody { get; set; }
        public string WatchId { get; set; }
    }

    public class NicoNicoMyListEntity : TableEntity
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public int ViewCounter { get; set; }
        public int MyListCounter { get; set; }
        public int CommentCounter { get; set; }

        public DateTime InsertedTime { get; set; }

        public NicoNicoMyListEntity()
        {
        }

        public NicoNicoMyListEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
