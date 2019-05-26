using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repositories
{
    public class SleepDataRepository : StorageTableRepository<SleepDataEntity>
    {
        public SleepDataRepository(ICloudStorageAccountProvider cloudStorageAccountProvider)
            : base(cloudStorageAccountProvider, "sleepdata")
        {
        }

        public async Task<SleepDataEntity[]> GetByDate(DateTime from, DateTime to)
        {
            var propertyName = nameof(SleepDataEntity.OutofBedDateandTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<SleepDataEntity>().Where(finalFilter);

            var items = await CloudTable
                .ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            var result = items
                .OrderBy(x => x.OutofBedDateandTime)
                .ToArray();

            return result;
        }
    }

    public class SleepDataEntity : TableEntity
    {
        public string SleepDuration { get; set; }
        public string DeviceUser { get; set; }
        public DateTime InBedDateandTime { get; set; }
        public string DeviceMac { get; set; }
        public DateTime OutofBedDateandTime { get; set; }
        public string LightSleepDuration { get; set; }
        public string DeepSleepDuration { get; set; }
        public string RemSleepDuration { get; set; }
        public int SleepScore { get; set; }
        public string SnoringDuration { get; set; }
        public int SnoringEpisodesCount { get; set; }
        public int HeartRateAverage { get; set; }
        public string AwakeDuration { get; set; }
        public string StatusSleepScore { get; set; }
        public string StatusRegularity { get; set; }
        public int NbInterruptions { get; set; }
        public string TimeToSleep { get; set; }
        public string TimeToGetUp { get; set; }
        public DateTime InsertedTime { get; set; }

        public SleepDataEntity()
        {
        }

        public SleepDataEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }
}
