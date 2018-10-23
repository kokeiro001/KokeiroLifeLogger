using EnsureThat;
using KokeiroLifeLogger.Repositories;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IIFTTTService
    {
        Task AddData(IFTTTEntity entity);
        Task<IFTTTEntity[]> GetDataByDate(DateTime from, DateTime to);
    }

    public class IFTTTService : IIFTTTService
    {
        private readonly IIFTTTRepository iftttRepository;

        public IFTTTService(
            IIFTTTRepository iftttRepository
        )
        {
            this.iftttRepository = iftttRepository;
        }

        public async Task AddData(IFTTTEntity entity)
        {
            await iftttRepository.AddAsync(entity);
        }

        public Task<IFTTTEntity[]> GetDataByDate(DateTime from, DateTime to)
        {
            return iftttRepository.GetByDate(from, to);
        }
    }
}
