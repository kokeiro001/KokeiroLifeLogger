using KokeiroLifeLogger.Services;
using KokeiroLifeLogger.Utilities;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repositories
{
    public class WeightMesurementEntity : TableEntity
    {
        public double Weight { get; set; }
        public double LeanMass { get; set; }
        public double FatMass { get; set; }
        public double FatPercent { get; set; }
        public DateTime MesuredAt { get; set; }
        public DateTime InsertedTime { get; set; }

        public WeightMesurementEntity()
            : base("WeightMesurement", DateTime.UtcNow.Ticks.ToString())
        {
        }

        public static WeightMesurementEntity Parse(string jsonStr)
        {
            var json = JObject.Parse(jsonStr);

            return new WeightMesurementEntity()
            {
                Weight = (double)json["WeightKg"],
                LeanMass = (double)json["LeanMassKg"],
                FatMass = (double)json["FatMassKg"],
                FatPercent = (double)json["FatPercent"],
                MesuredAt = DateTimeParser.ParseWithingsDate((string)json["MeasuredAt"]).AddHours(9), // jct -> utc
            };
        }
    }

    public interface IWeightMeasurementRepository : IStorageTableRepository<WeightMesurementEntity>
    {
        Task<WeightMesurementEntity> GetByDateDateTime(DateTime from, DateTime to);
    }

    public class WeightMeasurementRepository : StorageTableRepository<WeightMesurementEntity>, IWeightMeasurementRepository
    {
        public WeightMeasurementRepository(ICloudStorageAccountProvider cloudStorageAccountProvider) 
            : base(cloudStorageAccountProvider, @"weightmeasurement")
        {
        }

        public async Task<WeightMesurementEntity> GetByDateDateTime(DateTime from, DateTime to)
        {
            await CloudTable.CreateIfNotExistsAsync();

            var propertyName = nameof(WeightMesurementEntity.MesuredAt);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<WeightMesurementEntity>().Where(finalFilter);
            var items = await CloudTable.ExecuteQuerySegmentedAsync(query, new TableContinuationToken());
            
            var result = items.FirstOrDefault();

            return result;
        }
    }
}
