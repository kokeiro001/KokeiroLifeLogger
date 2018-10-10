using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Text;
using System.Collections.Generic;
using KokeiroLifeLogger.Utilities;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using Newtonsoft.Json;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class IFTTTHttpTrigger
    {
        public static string TableName = @"ifttt";

        [FunctionName("IFTTTHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "ifttt")]HttpRequestMessage req,
            ILogger logger,
            [Inject]IIFTTTService iftttService
        )
        {
            var json = await req.Content.ReadAsStringAsync();

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