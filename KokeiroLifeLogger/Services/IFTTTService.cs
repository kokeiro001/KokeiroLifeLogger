using AzureFunctions.Autofac;
using KokeiroLifeLogger.Functions;
using KokeiroLifeLogger.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IIFTTTService
    {
        Task AddData(IFTTTEntity entity);
        IFTTTEntity[] GetDataByDate(DateTime from, DateTime to);
    }

    public class IFTTTService : IIFTTTService
    {
        private readonly IIFTTTRepository iftttRepository;

        public IFTTTService(
            [Inject] IIFTTTRepository iftttRepository
        )
        {
            this.iftttRepository = iftttRepository;
        }

        public async Task AddData(IFTTTEntity entity)
        {
            await iftttRepository.AddAsync(entity);
        }

        public IFTTTEntity[] GetDataByDate(DateTime from, DateTime to)
        {
            return iftttRepository.GetByDate(from, to);
        }
    }
}
