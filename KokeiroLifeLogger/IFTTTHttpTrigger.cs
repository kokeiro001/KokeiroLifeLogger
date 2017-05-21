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


            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();


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