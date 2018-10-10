using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Text;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Repository;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class WeightMeasurementTrigger
    {
        public static string TableName = @"weightmeasurement";

        [FunctionName("WeightMeasurementTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "bodymesurement")]HttpRequestMessage req,
            ILogger logger,
            [Inject]IWeightMeasurementService weightMeasurementService
        )
        {
            var jsonStr = await req.Content.ReadAsStringAsync();
            logger.LogInformation(jsonStr);

            var item = WeightMesurementEntity.Parse(jsonStr);
            item.InsertedTime = DateTime.UtcNow;

            logger.LogInformation($"weight={item.Weight}, leanMass={item.LeanMass}, fatMass={item.FatMass}, fatPercent={item.FatPercent}, mesuredAt={item.MesuredAt}");

            var table = await GetCloudTableAsync();

            var op = TableOperation.Insert(item);

            await table.ExecuteAsync(op);
            return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(item));
        }

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccountUtility.GetDefaultStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}