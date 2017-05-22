using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;

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

        public static async Task<string> GetData(DateTime from, DateTime to)
        {
            var table = await GetCloudTableAsync();

            var query = new TableQuery<IFTTTEntity>()
                            .Where(TableQuery.GenerateFilterConditionForDate("InsertedItme", QueryComparisons.GreaterThanOrEqual, from));
            var result = table.ExecuteQuery(query);

            var sb = new StringBuilder();

            sb.AppendLine("-------------------------------------");
            sb.AppendLine("今日Pocketに突っ込んだ記事");
            sb.AppendLine();
            foreach (var item in result)
            {
                sb.AppendLine(item.Title);
                sb.AppendLine(item.Url);
                sb.AppendLine();
            }
            sb.AppendLine("-------------------------------------");
            return sb.ToString();
        }

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
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