using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Repository
{
    public interface IStorageTableRepository<T> where T : TableEntity
    {
        Task AddAsync(T entity);
    }

    public abstract class StorageTableRepository<T> : TableEntity, IStorageTableRepository<T> where T : TableEntity
    {
        protected CloudTable CloudTable { get; }

        public StorageTableRepository(ICloudStorageAccountProvider cloudStorageAccountProvider, string tableName)
        {
            var cloudStorageAccount = cloudStorageAccountProvider.GetCloudStorageAccount();
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable = tableClient.GetTableReference(tableName);
        }

        public async Task AddAsync(T entity)
        {
            await CloudTable.CreateIfNotExistsAsync();
            var op = TableOperation.InsertOrReplace(entity);
            await CloudTable.ExecuteAsync(op);
        }
    }
}
