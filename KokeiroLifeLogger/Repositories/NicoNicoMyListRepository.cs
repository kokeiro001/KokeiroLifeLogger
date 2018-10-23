using KokeiroLifeLogger.Services;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace KokeiroLifeLogger.Repository
{

    public class NicoNicoMyListEntity : TableEntity
    {
        public string Title { get; set; }
        public string VideoId { get; set; }
        public int ViewCounter { get; set; }
        public int MyListCounter { get; set; }
        public int CommentCounter { get; set; }

        public DateTime InsertedTime { get; set; }

        public NicoNicoMyListEntity()
        {
        }

        public NicoNicoMyListEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
    }

    public interface INicoNicoMyListRepository : IStorageTableRepository<NicoNicoMyListEntity>
    {
    }

    public class NicoNicoMyListRepository : StorageTableRepository<NicoNicoMyListEntity>, INicoNicoMyListRepository
    {
        public NicoNicoMyListRepository(ICloudStorageAccountProvider cloudStorageAccountProvider) 
            : base(cloudStorageAccountProvider, @"nicoNicoMyList")
        {
        }
    }
}
