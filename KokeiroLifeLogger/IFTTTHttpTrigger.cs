using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;
using System.Collections.Generic;
using KokeiroLifeLogger.Utilities;

namespace KokeiroLifeLogger
{
    public static class IFTTTHttpTrigger
    {
        public static string TableName = @"ifttt";

        [FunctionName("IFTTTHttpTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ifttt")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var jsonStr = await req.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonStr);

            var title = (string)json["title"];
            var url = (string)json["url"];
            var from = (string)json["from"];

            log.Info($"title={title}, url={url}, from={from}");

            var table = await GetCloudTableAsync();

            var entity = new IFTTTEntity(from, url.GetHashCode().ToString())
            {
                Title = title,
                Url = url,
                InsertedItme = DateTime.Now,
            };

            TableOperation insertOperation = TableOperation.Insert(entity);
            try
            {
                await table.ExecuteAsync(insertOperation);
                return req.CreateResponse(HttpStatusCode.OK, $"Title={title} Url={url}");
            }
            catch (Exception e)
            {
                log.Error(e.Message + " " + e.StackTrace);
                throw;
            }
        }

        #region Output

        public static async Task<string> GetDataAsync(DateTime from, DateTime to)
        {
            var table = await GetCloudTableAsync();

            var propertyName = nameof(IFTTTEntity.InsertedItme);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<IFTTTEntity>().Where(finalFilter);
            var items = table.ExecuteQuery(query).OrderBy(x => x.InsertedItme);

            var sb = new StringBuilder();

            AppendStringByPartitionKey(items, sb, "pocket", "今日Pocketに突っ込んだ記事");
            AppendStringTwitterLiked(items, sb);
            AppendStringByPartitionKey(items, sb, "github_star", "今日GitHubでStarつけたリポジトリ");
            return sb.ToString();
        }

        private static void AppendStringByPartitionKey(IEnumerable<IFTTTEntity> items, StringBuilder sb, string key, string title)
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
        private static void AppendStringTwitterLiked(IEnumerable<IFTTTEntity> items, StringBuilder sb)
        {
            sb.AppendLine("-------------------------------------");
            sb.AppendLine("今日Twitterでイイねしたツイート");
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

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        #endregion


    }

    public class IFTTTEntity: TableEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime InsertedItme { get; set; }

        public IFTTTEntity()
        {
        }

        public IFTTTEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}