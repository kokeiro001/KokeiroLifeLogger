using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Utilities;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class LocationEnteredOrExitedHttpTrigger
    {
        public static string TableName = @"locationEnteredOrExited";

        [FunctionName("LocationEnteredOrExitedHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "location")]HttpRequestMessage req,
            ILogger logger,
            [Inject]ILocationEnteredOrExitedService locationEnteredOrExitedService
        )
        {
            var json = await req.Content.ReadAsStringAsync();

            var request = JsonConvert.DeserializeObject<LocationEnteredOrExitedRequest>(json);

            logger.LogInformation($"location={request.Location}, enteredOrExited={request.EnteredOrExited}");

            var now = DateTime.UtcNow;

            var entity = new LocationEnteredOrExitedEntity(request.Location, now.Ticks.ToString())
            {
                Location = request.Location,
                EnteredOrExited = request.EnteredOrExited,
                CreatedAt = now,
            };

            await locationEnteredOrExitedService.AddAsync(entity);

            return req.CreateResponse(HttpStatusCode.OK, $"Location={request.Location} EnteredOrExited={request.EnteredOrExited}");
        }
    }

    public class LocationEnteredOrExitedRequest
    {
        public string Location { get; set; }
        public string EnteredOrExited { get; set; }
    }
}
