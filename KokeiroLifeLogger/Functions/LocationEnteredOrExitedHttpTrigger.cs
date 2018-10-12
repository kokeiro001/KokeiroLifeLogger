using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Injection;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class LocationEnteredOrExitedHttpTrigger
    {
        [FunctionName("LocationEnteredOrExitedHttpTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "location")]HttpRequestMessage req,
            ILogger logger,
            [Inject]ILocationEnteredOrExitedService locationEnteredOrExitedService
        )
        {
            var json = await req.Content.ReadAsStringAsync();
            logger.LogInformation(json);

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
