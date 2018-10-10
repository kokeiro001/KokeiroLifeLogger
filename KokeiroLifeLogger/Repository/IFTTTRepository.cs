using KokeiroLifeLogger.Functions;
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
        IFTTTEntity[] GetByDate(DateTime from, DateTime to);
    }

    public class IFTTTRepository : StorageTableRepository<IFTTTEntity>, IIFTTTRepository
    {
        public IFTTTRepository(ICloudStorageAccountProvider cloudStorageAccountProvider)
            : base(cloudStorageAccountProvider.GetCloudStorageAccount(), "ifttt")
        {
        }

        public IFTTTEntity[] GetByDate(DateTime from, DateTime to)
        {
            var propertyName = nameof(IFTTTEntity.InsertedTime);
            var filter1 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.GreaterThanOrEqual, from);
            var filter2 = TableQuery.GenerateFilterConditionForDate(propertyName, QueryComparisons.LessThanOrEqual, to);

            var finalFilter = TableQuery.CombineFilters(filter1, "and", filter2);

            var query = new TableQuery<IFTTTEntity>().Where(finalFilter);
            var items = CloudTable
                .ExecuteQuery(query)
                .OrderBy(x => x.InsertedTime)
                .ToArray();

            return items;
        }
    }
}
