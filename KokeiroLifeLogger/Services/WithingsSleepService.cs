using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KokeiroLifeLogger.Repository;

namespace KokeiroLifeLogger.Services
{
    public interface IWithingsSleepService
    {
        Task AddAsync(WithingsSleepEntity entity);
    }

    public class WithingsSleepService : IWithingsSleepService
    {
        private readonly IWithingsSleepRepository withingsSleepRepository;

        public WithingsSleepService(
            IWithingsSleepRepository withingsSleepRepository
        )
        {
            this.withingsSleepRepository = withingsSleepRepository;
        }

        public async Task AddAsync(WithingsSleepEntity entity)
        {
            await withingsSleepRepository.AddAsync(entity);
        }
    }
}
