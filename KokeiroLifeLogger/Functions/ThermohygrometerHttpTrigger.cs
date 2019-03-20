using AzureFunctions.Autofac;
using KokeiroLifeLogger.Repositories;
using KokeiroLifeLogger.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class ThermohygrometerHttpTrigger
    {
        [FunctionName("ThermohygrometerHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "thermohygrometer")] HttpRequest req,
            ILogger logger,
            [Inject]IThermohygrometerService thermohygrometerService
        )
        {
            var json = await req.ReadAsStringAsync();
            logger.LogInformation(json);

            var requestData = JsonConvert.DeserializeObject<RequestThermohygrometer>(json);

            var info = $"Temperature={requestData.Temperature}, Humidity={requestData.Humidity}, Location={requestData.Location}, MesuredAt={requestData.MesuredAt}";
            logger.LogInformation(info);

            var entity = new ThermohygrometerEntity(requestData.Location, requestData.MesuredAt.Ticks.ToString().ToString())
            {
                Location = requestData.Location,
                Temperature = requestData.Temperature,
                Humidity = requestData.Humidity,
                MesuredAt = requestData.MesuredAt,
            };

            await thermohygrometerService.AddData(entity);

            return new OkObjectResult(info);
        }

        private class RequestThermohygrometer
        {
            public string Location { get; set; }
            public double Temperature { get; set; }
            public double Humidity { get; set; }
            public DateTime MesuredAt { get; set; }
        }
    }
}
