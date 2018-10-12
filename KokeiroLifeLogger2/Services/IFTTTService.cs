using Autofac;
using KokeiroLifeLogger.Repository;
using Microsoft.Azure.WebJobs.Host.Config;
using System;
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
            IIFTTTRepository iftttRepository
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


    public class InjectConfiguration : IExtensionConfigProvider
    {
        private static readonly object _syncLock = new object();
        private static IContainer _container;

        public void Initialize(ExtensionConfigContext context)
        {
            InitializeContainer(context);

            //context
            //    .AddBindingRule<InjectaAttribute>()
            //    .BindToInput<dynamic>(i => _container.Resolve(i.Type));
        }

        private void InitializeContainer(ExtensionConfigContext context)
        {
            if (_container != null)
            {
                return;
            }

            lock (_syncLock)
            {
                if (_container != null)
                {
                    return;
                }

                //context.Config.LoggerFactory;
            }
        }
    }
}
