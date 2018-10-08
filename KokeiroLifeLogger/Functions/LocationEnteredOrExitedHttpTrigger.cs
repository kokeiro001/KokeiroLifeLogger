using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using KokeiroLifeLogger.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;

namespace KokeiroLifeLogger.Functions
{
    public static class LocationEnteredOrExitedHttpTrigger
    {
        public static string TableName = @"locationEnteredOrExited";

        [FunctionName("LocationEnteredOrExitedHttpTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "location")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var jsonStr = await req.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonStr);
            log.Info($"jsonStr={jsonStr}");

            var location = (string)json["location"];
            var enteredOrExited = (string)json["enteredOrExited"];

            log.Info($"location={location}, enteredOrExited={enteredOrExited}");

            var table = await GetCloudTableAsync();

            var now = DateTime.UtcNow;

            var entity = new LocationEnteredOrExitedEntity(location, now.Ticks.ToString())
            {
                Location = location,
                EnteredOrExited = enteredOrExited,
                CreatedAt = now,
            };

            var op = TableOperation.InsertOrReplace(entity);
            try
            {
                await table.ExecuteAsync(op);
                return req.CreateResponse(HttpStatusCode.OK, $"Location={location} EnteredOrExited={enteredOrExited}");
            }
            catch (Exception e)
            {
                log.Error(e.Message + " " + e.StackTrace);
                throw;
            }
        }

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccountUtility.GetDefaultStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        public class LocationEnteredOrExitedEntity : TableEntity
        {
            public string Location { get; set; }

            public string EnteredOrExited { get; set; }

            public DateTime CreatedAt { get; set; }

            public LocationEnteredOrExitedEntity()
            {
            }

            public LocationEnteredOrExitedEntity(string partitionKey, string rowKey)
            {
                PartitionKey = partitionKey;
                RowKey = rowKey;
            }
        }
    }

}
