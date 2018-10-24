using KokeiroLifeLogger.Services;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace KokeiroLifeLogger.Test
{
    public class NicoNicoMyListServiceTest
    {
        [Fact]
        public async Task TestMethod1()
        {
            var nicoNicoMyListObserveService = new NicoNicoMyListObserveService(null);

            var items = await nicoNicoMyListObserveService.GetMyListItems(63412739);

            items.IsNotNull();
            items.Any().IsTrue();
        }
    }
}
