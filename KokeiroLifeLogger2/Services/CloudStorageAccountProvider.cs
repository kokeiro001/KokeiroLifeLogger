using Microsoft.WindowsAzure.Storage;

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
