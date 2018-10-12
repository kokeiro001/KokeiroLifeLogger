using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Injection;
using AzureFunctions.Autofac;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class IFTTTHttpTrigger
    {
        [FunctionName("IFTTTHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ifttt")]HttpRequestMessage req,
            ILogger logger,
            [Inject]IIFTTTService iftttService
        )
        {
            var json = await req.Content.ReadAsStringAsync();
            logger.LogInformation(json);

            var requestData = JsonConvert.DeserializeObject<IFTTTRequestData>(json);

            logger.LogInformation($"title={requestData.Title}, url={requestData.Url}, from={requestData.From}");

            var entity = new IFTTTEntity(requestData.From, requestData.Url.GetHashCode().ToString())
            {
                Title = requestData.Title,
                Url = requestData.Url,
                InsertedTime = DateTime.UtcNow,
            };

            await iftttService.AddData(entity);

            return req.CreateResponse(HttpStatusCode.OK, $"Title={requestData.Title} Url={requestData.Url}");
        }
    }

    public class IFTTTRequestData
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public string From { get; set; }
    }

}