using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repository
{
    public class WithingsSleepEntity : TableEntity
    {
        public string Action { get; set; }
        public DateTime Date { get; set; }
        public DateTime InsertedTime { get; set; }

        public WithingsSleepEntity()
        {
        }

        public WithingsSleepEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }

    public interface IWithingsSleepRepository : IStorageTableRepository<WithingsSleepEntity>
    {
        Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to);
        Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to);
    }

    public class WithingsSleepRepository : StorageTableRepository<WithingsSleepEntity>, IWithingsSleepRepository
    {
        public WithingsSleepRepository(ICloudStorageAccountProvider cloudStorageAccountProvider) 
            : base(cloudStorageAccountProvider, "withingssleep")
        {
        }


        private async Task<WithingsSleepEntity[]> GetByDate(DateTime from, DateTime to)
        {
            var propertyName = nameof(WithingsSleepEntity.InsertedTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<WithingsSleepEntity>().Where(finalFilter);
            var items = await CloudTable
                .ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            var result = items
                .OrderBy(x => x.InsertedTime)
                .ToArray();

            return result;
        }

        public async Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to)
        {
            var data = await GetByDate(from, to);
            return data
                .Where(x => x.Action == "into bed")
                .ToArray();
        }

        public async Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to)
        {
            var data = await GetByDate(from, to);
            return data
                .Where(x => x.Action == "out bed")
                .ToArray();
        }
    }
}
