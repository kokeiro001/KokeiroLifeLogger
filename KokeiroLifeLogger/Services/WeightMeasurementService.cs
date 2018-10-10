using KokeiroLifeLogger.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IWeightMeasurementService
    {
        Task AddData(WeightMesurementEntity entity);
        WeightMesurementEntity GetByDate(DateTime from, DateTime to);
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

        public WeightMesurementEntity GetByDate(DateTime from, DateTime to)
        {
            return weightMeasurementRepository.GetByDateDateTime(from, to);
        }
    }
}
