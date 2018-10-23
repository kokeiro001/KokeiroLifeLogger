using KokeiroLifeLogger.Repositories;
using System;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IWeightMeasurementService
    {
        Task AddData(WeightMesurementEntity entity);
        Task<WeightMesurementEntity> GetByDate(DateTime from, DateTime to);
    }

    public class WeightMeasurementService : IWeightMeasurementService
    {
        private readonly IWeightMeasurementRepository weightMeasurementRepository;

        public WeightMeasurementService(
            IWeightMeasurementRepository weightMeasurementRepository
        )
        {
            this.weightMeasurementRepository = weightMeasurementRepository;
        }

        public async Task AddData(WeightMesurementEntity entity)
        {
            await weightMeasurementRepository.AddAsync(entity);
        }

        public Task<WeightMesurementEntity> GetByDate(DateTime from, DateTime to)
        {
            return weightMeasurementRepository.GetByDateDateTime(from, to);
        }
    }
}
