using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

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
    }

    public class WithingsSleepRepository : StorageTableRepository<WithingsSleepEntity>, IWithingsSleepRepository
    {
        public WithingsSleepRepository(ICloudStorageAccountProvider cloudStorageAccountProvider) 
            : base(cloudStorageAccountProvider, "withingssleep")
        {
        }
    }
}
