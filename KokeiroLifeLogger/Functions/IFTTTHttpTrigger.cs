using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;
using System.Collections.Generic;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using Newtonsoft.Json;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class IFTTTHttpTrigger
    {
        public static string TableName = @"ifttt";

        [FunctionName("IFTTTHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ifttt")]HttpRequestMessage req,
            ILogger logger
        )
        {
            var json = await req.Content.ReadAsStringAsync();

            var requestData = JsonConvert.DeserializeObject<IFFFTRequestData>(json);

            logger.LogInformation($"title={requestData.Title}, url={requestData.Url}, from={requestData.From}");

            var table = await GetCloudTableAsync();

            var entity = new IFTTTEntity(requestData.From, requestData.Url.GetHashCode().ToString())
            {
                Title = requestData.Title,
                Url = requestData.Url,
                InsertedTime = DateTime.UtcNow,
            };

            var op = TableOperation.InsertOrReplace(entity);
            await table.ExecuteAsync(op);
            return req.CreateResponse(HttpStatusCode.OK, $"Title={requestData.Title} Url={requestData.Url}");
        }

        // TODO: 別のクラスに分離する
        #region Output

        public static async Task<string> GetDataAsync(DateTime from, DateTime to)
        {
            var table = await GetCloudTableAsync();

            var propertyName = nameof(IFTTTEntity.InsertedTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<IFTTTEntity>().Where(finalFilter);
            var items = table.ExecuteQuery(query).OrderBy(x => x.InsertedTime);

            var sb = new StringBuilder();

            AppendStringByPartitionKey(items, sb, "pocket", "Pocketに突っ込んだ記事");
            AppendStringTwitterLiked(items, sb);
            AppendStringByPartitionKey(items, sb, "github_star", "GitHubでStarつけたリポジトリ");
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

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccountUtility.GetDefaultStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        #endregion


    }

    public class IFFFTRequestData
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string From { get; set; }
    }

    public class IFTTTEntity: TableEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime InsertedTime { get; set; }

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