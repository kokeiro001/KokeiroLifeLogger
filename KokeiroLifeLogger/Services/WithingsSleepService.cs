using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using KokeiroLifeLogger.Repositories;

namespace KokeiroLifeLogger.Services
{
    public interface IWithingsSleepService
    {
        Task AddAsync(WithingsSleepEntity entity);
        Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to);
        Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to);
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

        public Task<WithingsSleepEntity[]> GetIntoBedDataByDate(DateTime from, DateTime to)
        {
            return withingsSleepRepository.GetIntoBedDataByDate(from, to);
        }

        public Task<WithingsSleepEntity[]> GetOutBedDataByDate(DateTime from, DateTime to)
        {
            return withingsSleepRepository.GetOutBedDataByDate(from, to);
        }
    }
}
