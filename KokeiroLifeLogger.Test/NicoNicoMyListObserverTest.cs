using KokeiroLifeLogger.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Test
{
    [TestClass]
    public sealed class NicoNicoMyListObserverTest
    {
        [DataTestMethod]
        [DataRow(63412739, 4)]
        public async Task パースできる(int myListId, int minMyListItemCount)
        {
            var service = new NicoNicoMyListObserveService(CloudStorageAccount.DevelopmentStorageAccount);

            var myListItems = await service.GetMyListItems(myListId);
            var existItems = myListItems.Length >= minMyListItemCount;

            Assert.AreEqual(existItems, true);
        }

        [DataTestMethod]
        [DataRow(63412739, 4)]
        public async Task パースして保存できる(int myListId, int minMyListItemCount)
        {
            var service = new NicoNicoMyListObserveService(CloudStorageAccount.DevelopmentStorageAccount);

            var myListItems = await service.GetMyListItems(myListId);
            var existItems = myListItems.Length >= minMyListItemCount;

            Assert.AreEqual(existItems, true);

            await service.SaveMyListItems(myListItems);
        }
    }
}
