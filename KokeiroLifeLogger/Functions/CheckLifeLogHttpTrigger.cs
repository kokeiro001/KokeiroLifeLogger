using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using KokeiroLifeLogger.Utilities;
using KokeiroLifeLogger.Services;
using Microsoft.Extensions.Logging;

namespace KokeiroLifeLogger.Functions
{
    public static class CheckLifeLogHttpTrigger
    {
        [FunctionName("CheckLifeLog")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]HttpRequestMessage req,
            ILogger logger
        )
        {
            var lifeLog = await new LifeLogCrawler(logger).CrawlAsync();
            var body = $"{lifeLog.Title}\n\n{lifeLog.Body}";
            return req.CreateResponse(HttpStatusCode.OK, body);
        }
    }
}