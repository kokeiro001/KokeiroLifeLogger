using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repositories
{
    public class IFTTTEntity : TableEntity
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public DateTime InsertedTime { get; set; }

        public IFTTTEntity()
        {
        }

        public IFTTTEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }

    public interface IIFTTTRepository : IStorageTableRepository<IFTTTEntity>
    {
        Task<IFTTTEntity[]> GetByDate(DateTime from, DateTime to);
    }

    public class IFTTTRepository : StorageTableRepository<IFTTTEntity>, IIFTTTRepository
    {
        public IFTTTRepository(ICloudStorageAccountProvider cloudStorageAccountProvider)
            : base(cloudStorageAccountProvider, "ifttt")
        {
        }

        public async Task<IFTTTEntity[]> GetByDate(DateTime from, DateTime to)
        {
            var propertyName = nameof(IFTTTEntity.InsertedTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<IFTTTEntity>().Where(finalFilter);
            var items = await CloudTable
                .ExecuteQuerySegmentedAsync(query, new TableContinuationToken());

            var result = items
                .OrderBy(x => x.InsertedTime)
                .ToArray();

            return result;
        }
    }
}
