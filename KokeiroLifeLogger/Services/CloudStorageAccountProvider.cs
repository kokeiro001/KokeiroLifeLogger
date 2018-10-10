using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface ICloudStorageAccountProvider
    {
        CloudStorageAccount GetCloudStorageAccount();
    }

    public class CloudStorageAccountProvider : ICloudStorageAccountProvider
    {
        private readonly CloudStorageAccount cloudStorageAccount;

        public CloudStorageAccountProvider(CloudStorageAccount cloudStorageAccount)
        {
            this.cloudStorageAccount = cloudStorageAccount;
        }

        public CloudStorageAccount GetCloudStorageAccount()
        {
            return cloudStorageAccount;
        }
    }
}
