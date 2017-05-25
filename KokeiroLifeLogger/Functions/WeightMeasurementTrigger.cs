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
using System.Text;

namespace KokeiroLifeLogger.Functions
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
            item.InsertedTime = DateTime.UtcNow;

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

        // TODO: 別のクラスに分離する
        public static async Task<string> GetDataAsync(DateTime from, DateTime to)
        {
            var table = await GetCloudTableAsync();

            var propertyName = nameof(WeightMesurement.InsertedTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<WeightMesurement>().Where(finalFilter);
            var item = table.ExecuteQuery(query).FirstOrDefault();

            var sb = new StringBuilder();
            sb.AppendLine("-------------------------------------");

            if (item != null)
            {
                sb.AppendLine($"体重:{item.Weight}kg");
                sb.AppendLine($"除脂肪体重:{item.LeanMass}kg");
                sb.AppendLine($"体脂肪量:{item.FatMass}kg");
                sb.AppendLine($"体脂肪率:{item.FatPercent}%");
            }
            else
            {
                sb.AppendLine($"(今日は体重計乗ってないらしい)");
            }

            sb.AppendLine();
            return sb.ToString();
        }

        class WeightMesurement : TableEntity
        {
            public double Weight { get; set; }
            public double LeanMass { get; set; }
            public double FatMass { get; set; }
            public double FatPercent { get; set; }
            public DateTime MesuredAt { get; set; }
            public DateTime InsertedTime { get; set; }

            public WeightMesurement() 
                : base("WeightMesurement", DateTime.UtcNow.Ticks.ToString())
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