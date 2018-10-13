
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using KokeiroLifeLogger.Utilities;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using AzureFunctions.Autofac;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class WithingsSleepFunction
    {
        [FunctionName("withingssleep")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequest req, 
            ILogger logger,
            [Inject]IWithingsSleepService withingsSleepService
        )
        {
            logger.LogInformation("C# HTTP trigger function processed a request.");

            var json = await req.ReadAsStringAsync();
            logger.LogInformation(json);

            var withingsSleepRequest = JsonConvert.DeserializeObject<WighingsSleepRequest>(json);

            var date = DateTimeParser.ParseWithingsDate(withingsSleepRequest.DateAndTime);

            var now = DateTime.UtcNow;
            var entity = new WithingsSleepEntity("withingsSleepEntity", now.Ticks.ToString())
            {
                Action = withingsSleepRequest.Action,
                Date = date,
                InsertedTime = now,
            };

            logger.LogInformation($"action={entity.Action}, date={entity.Date}");

            await withingsSleepService.AddAsync(entity);

            return new OkObjectResult($"action={entity.Action}, date={entity.Date}");
        }
    }

    public class WighingsSleepRequest
    {
        public string Action { get; set; }
        public string DateAndTime { get; set; }
    }
}
