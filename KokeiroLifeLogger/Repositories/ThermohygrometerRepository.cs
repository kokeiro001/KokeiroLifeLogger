using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace KokeiroLifeLogger.Repositories
{
    public class ThermohygrometerEntity : TableEntity
    {
        public string Location { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        public DateTime MesuredAt { get; internal set; }

        public ThermohygrometerEntity()
        {
        }

        public ThermohygrometerEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }

    public interface IThermohygrometerRepository : IStorageTableRepository<ThermohygrometerEntity>
    {
    }

    class ThermohygrometerRepository : StorageTableRepository<ThermohygrometerEntity>, IThermohygrometerRepository
    {
        public ThermohygrometerRepository(ICloudStorageAccountProvider cloudStorageAccountProvider)
            : base(cloudStorageAccountProvider, "thermohygrometer")
        {
        }
    }
}
