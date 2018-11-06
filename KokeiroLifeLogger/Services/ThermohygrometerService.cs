using KokeiroLifeLogger.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger.Services
{
    public interface IThermohygrometerService
    {
        Task AddData(ThermohygrometerEntity entity);
    }

    class ThermohygrometerService : IThermohygrometerService
    {
        private readonly IThermohygrometerRepository thermohygrometerRepository;

        public ThermohygrometerService(
            IThermohygrometerRepository thermohygrometerRepository
        )
        {
            this.thermohygrometerRepository = thermohygrometerRepository;
        }

        public Task AddData(ThermohygrometerEntity entity)
        {
            return thermohygrometerRepository.AddAsync(entity);
        }
    }
}
