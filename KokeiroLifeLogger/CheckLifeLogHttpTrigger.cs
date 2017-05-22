using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace KokeiroLifeLogger
{
    public static class CheckLifeLogHttpTrigger
    {
        [FunctionName("CheckLifeLog")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var lifeLog = await new LifeLogCrawler().CrawlAsync();
            var body = $"{lifeLog.Title}\n\n{lifeLog.Body}";
            return req.CreateResponse(HttpStatusCode.OK, body);
        }
    }
}