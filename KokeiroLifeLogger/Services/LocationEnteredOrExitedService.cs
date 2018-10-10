using KokeiroLifeLogger.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface ILocationEnteredOrExitedService
    {
        Task AddAsync(LocationEnteredOrExitedEntity entity);
    }

    public class LocationEnteredOrExitedService : ILocationEnteredOrExitedService
    {
        private readonly ILocationEnteredOrExitedRepository locationEnteredOrExitedRepository;

        public LocationEnteredOrExitedService(
            ILocationEnteredOrExitedRepository locationEnteredOrExitedRepository
        )
        {
            this.locationEnteredOrExitedRepository = locationEnteredOrExitedRepository;
        }

        public async Task AddAsync(LocationEnteredOrExitedEntity entity)
        {
            await locationEnteredOrExitedRepository.AddAsync(entity);
        }
    }
}
