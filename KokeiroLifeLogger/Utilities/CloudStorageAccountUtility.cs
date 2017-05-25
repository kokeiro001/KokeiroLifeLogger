using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;

namespace KokeiroLifeLogger.Utilities
{
    static class CloudStorageAccountUtility
    {
        public static CloudStorageAccount GetDefaultStorageAccount() => 
            CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));
    }
}
