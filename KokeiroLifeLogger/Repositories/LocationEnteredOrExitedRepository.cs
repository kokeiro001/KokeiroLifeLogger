using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace KokeiroLifeLogger.Repositories
{
    public class LocationEnteredOrExitedEntity : TableEntity
    {
        public string Location { get; set; }

        public string EnteredOrExited { get; set; }

        public DateTime CreatedAt { get; set; }

        public LocationEnteredOrExitedEntity()
        {
        }

        public LocationEnteredOrExitedEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }

    public interface ILocationEnteredOrExitedRepository : IStorageTableRepository<LocationEnteredOrExitedEntity>
    {
    }

    class LocationEnteredOrExitedRepository : StorageTableRepository<LocationEnteredOrExitedEntity>, ILocationEnteredOrExitedRepository
    {
        public LocationEnteredOrExitedRepository(ICloudStorageAccountProvider cloudStorageAccountProvider) 
            : base(cloudStorageAccountProvider, "locationEnteredOrExited")
        {
        }
    }
}
