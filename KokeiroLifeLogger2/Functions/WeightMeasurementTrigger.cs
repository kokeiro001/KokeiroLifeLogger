using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Repository;

namespace KokeiroLifeLogger.Functions
{
    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class WeightMeasurementTrigger
    {
        [FunctionName("WeightMeasurementTrigger")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "bodymesurement")]HttpRequestMessage req,
            ILogger logger,
            [Inject]IWeightMeasurementService weightMeasurementService
        )
        {
            var json = await req.Content.ReadAsStringAsync();
            logger.LogInformation(json);

            var entity = WeightMesurementEntity.Parse(json);
            entity.InsertedTime = DateTime.UtcNow;

            logger.LogInformation($"weight={entity.Weight}, leanMass={entity.LeanMass}, fatMass={entity.FatMass}, fatPercent={entity.FatPercent}, mesuredAt={entity.MesuredAt}");

            await weightMeasurementService.AddData(entity);

            return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(entity));
        }
    }
}