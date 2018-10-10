using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repository
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
        public LocationEnteredOrExitedRepository(CloudStorageAccount cloudStorageAccount) 
            : base(cloudStorageAccount, "locationEnteredOrExited")
        {
        }
    }
}
