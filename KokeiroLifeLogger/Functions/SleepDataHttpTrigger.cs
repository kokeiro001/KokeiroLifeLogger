using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AzureFunctions.Autofac;
using KokeiroLifeLogger.Utilities;
using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Repositories;

namespace KokeiroLifeLogger.Functions
{

    [DependencyInjectionConfig(typeof(DIConfig))]
    public static class SleepDataHttpTrigger
    {
        [FunctionName("SleepDataHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "sleepdata")] HttpRequest req,
            ILogger logger,
            [Inject]SleepDataService sleepDataService
        )
        {
            logger.LogInformation("SleepDataHttpTrigger function processed a request.");

            var json = await req.ReadAsStringAsync();
            logger.LogInformation(json);

            var sleepDataRequest = JsonConvert.DeserializeObject<SleepDataRequest>(json);

            var now = DateTime.UtcNow;
            var entity = new SleepDataEntity("sleepDataEntity", now.Ticks.ToString())
            {
                SleepDuration = sleepDataRequest.SleepDuration.ToString(),
                DeviceUser = sleepDataRequest.DeviceUser,
                InBedDateandTime = DateTimeParser.ParseWithingsDate(sleepDataRequest.InBedDateandTime).AddHours(-9),
                DeviceMac = sleepDataRequest.DeviceMac,
                OutofBedDateandTime = DateTimeParser.ParseWithingsDate(sleepDataRequest.OutofBedDateandTime).AddHours(-9),
                LightSleepDuration = sleepDataRequest.LightSleepDuration.ToString(),
                DeepSleepDuration = sleepDataRequest.DeepSleepDuration.ToString(),
                RemSleepDuration = sleepDataRequest.RemSleepDuration.ToString(),
                SleepScore = sleepDataRequest.SleepScore,
                SnoringDuration = sleepDataRequest.SnoringDuration.ToString(),
                SnoringEpisodesCount = sleepDataRequest.SnoringEpisodesCount,
                HeartRateAverage = sleepDataRequest.HeartRateAverage,
                AwakeDuration = sleepDataRequest.AwakeDuration.ToString(),
                StatusSleepScore = sleepDataRequest.StatusSleepScore,
                StatusRegularity = sleepDataRequest.StatusRegularity,
                NbInterruptions = sleepDataRequest.NbInterruptions,
                TimeToSleep = sleepDataRequest.TimeToSleep.ToString(),
                TimeToGetUp = sleepDataRequest.TimeToGetUp.ToString(),
                InsertedTime = now,
            };

            await sleepDataService.AddAsync(entity);

            return new OkObjectResult("ok");
        }
    }

    public class SleepDataRequest
    {
        public TimeSpan SleepDuration { get; set; }
        public string DeviceUser { get; set; }
        public string InBedDateandTime { get; set; }
        public string DeviceMac { get; set; }
        public string OutofBedDateandTime { get; set; }
        public TimeSpan LightSleepDuration { get; set; }
        public TimeSpan DeepSleepDuration { get; set; }
        public TimeSpan RemSleepDuration { get; set; }
        public int SleepScore { get; set; }
        public TimeSpan SnoringDuration { get; set; }
        public int SnoringEpisodesCount { get; set; }
        public int HeartRateAverage { get; set; }
        public TimeSpan AwakeDuration { get; set; }
        public string StatusSleepScore { get; set; }
        public string StatusRegularity { get; set; }
        public int NbInterruptions { get; set; }
        public TimeSpan TimeToSleep { get; set; }
        public TimeSpan TimeToGetUp { get; set; }
    }
}
