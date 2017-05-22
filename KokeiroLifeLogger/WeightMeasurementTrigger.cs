using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json.Linq;
using System;

namespace KokeiroLifeLogger
{
    public static class WeightMeasurementTrigger
    {
        [FunctionName("WeightMeasurementTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "bodymesurement")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var jsonStr = await req.Content.ReadAsStringAsync();
            log.Info(jsonStr);

            var json = JObject.Parse(jsonStr);
            log.Info(json.ToString(Newtonsoft.Json.Formatting.Indented));

            var weight = (float)json["WeightKg"];
            var leanMass = (float)json["LeanMassKg"];
            var fatMass = (float)json["FatMassKg"];
            var fatPercent = (float)json["FatPercent"];
            var mesuredAtStr = (string)json["MeasuredAt"];
            var mesuredAt = DateTime.ParseExact(mesuredAtStr, "MMM dd, yyyy 'at' hh:mmtt", new System.Globalization.CultureInfo("en-US"));


            log.Info($"weight={weight}, leanMass={leanMass}, fatMass={fatMass}, fatPercent={fatPercent}, mesuredAt={mesuredAt}");

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}