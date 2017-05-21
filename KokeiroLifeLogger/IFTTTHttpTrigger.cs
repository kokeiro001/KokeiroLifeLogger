using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KokeiroLifeLogger
{
    public static class IFTTTHttpTrigger
    {
        [FunctionName("IFTTTHttpTrigger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "ifttt")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var jsonStr = await req.Content.ReadAsStringAsync();
            var json = JObject.Parse(jsonStr);

            var title = json["title"];
            var url = json["url"];

            return req.CreateResponse(HttpStatusCode.OK, $"Title={title} Url={url}");
        }
    }
}