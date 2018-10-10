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

        public StorageTableRepository(CloudStorageAccount cloudStorageAccount, string tableName)
        {
            var tableClient = cloudStorageAccount.CreateCloudTableClient();
            CloudTable = tableClient.GetTableReference(tableName);
        }

        public async Task AddAsync(T entity)
        {
            var op = TableOperation.InsertOrReplace(entity);
            await CloudTable.ExecuteAsync(op);
        }
    }
}
