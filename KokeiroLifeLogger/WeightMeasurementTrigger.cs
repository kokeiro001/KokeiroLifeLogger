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
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;

namespace KokeiroLifeLogger
{
    public static class WeightMeasurementTrigger
    {
        public static string TableName = @"weightmeasurement";

        [FunctionName("WeightMeasurementTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "bodymesurement")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var jsonStr = await req.Content.ReadAsStringAsync();
            log.Info(jsonStr);

            var item = WeightMesurement.Parse(jsonStr);

            log.Info($"weight={item.Weight}, leanMass={item.LeanMass}, fatMass={item.FatMass}, fatPercent={item.FatPercent}, mesuredAt={item.MesuredAt}");

            var table = await GetCloudTableAsync();

            var op = TableOperation.Insert(item);

            try
            {
                await table.ExecuteAsync(op);
                return req.CreateResponse(HttpStatusCode.OK,  JsonConvert.SerializeObject(item));
            }
            catch (Exception e)
            {
                log.Error(e.Message + " " + e.StackTrace);
                throw;
            }
        }

        private static async Task<CloudTable> GetCloudTableAsync()
        {
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(TableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        class WeightMesurement : TableEntity
        {
            public double Weight { get; set; }
            public double LeanMass { get; set; }
            public double FatMass { get; set; }
            public double FatPercent { get; set; }
            public DateTime MesuredAt { get; set; }

            public WeightMesurement() 
                : base("WeightMesurement", DateTime.Now.Ticks.ToString())
            {
            }

            public static WeightMesurement Parse(string jsonStr)
            {
                var json = JObject.Parse(jsonStr);

                return new WeightMesurement()
                {
                    Weight = (double)json["WeightKg"],
                    LeanMass = (double)json["LeanMassKg"],
                    FatMass = (double)json["FatMassKg"],
                    FatPercent = (double)json["FatPercent"],
                    MesuredAt = DateTime.ParseExact((string)json["MeasuredAt"], "MMM dd, yyyy 'at' hh:mmtt", new System.Globalization.CultureInfo("en-US")),
                };
            }
        }
    }
}