using EnsureThat;
using KokeiroLifeLogger.Repository;
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
        private readonly ILogger logger;

        public IFTTTService(
            IIFTTTRepository iftttRepository,
            ILogger logger
        )
        {
            this.iftttRepository = iftttRepository;
            this.logger = logger;
        }

        public async Task AddData(IFTTTEntity entity)
        {
            logger.LogInformation(entity.PartitionKey);
            logger.LogInformation(entity.Title);
            logger.LogInformation(entity.Url);
            await iftttRepository.AddAsync(entity);
        }

        public Task<IFTTTEntity[]> GetDataByDate(DateTime from, DateTime to)
        {
            return iftttRepository.GetByDate(from, to);
        }
    }

    /*

    [Binding]
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class InjectaAttribute : Attribute
    {
        public InjectaAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
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

                // JobHostConfiguration class
                // ILoggingFactory
                //new LoggingFactory
                ILoggerFactory.CreateLogger.
                _container = ContainerConfig.BuildContainer(context.Config.LoggerFactory);
            }
        }
    }

    public static class ContainerConfig
    {
        public static IContainer BuildContainer(ILoggerFactory factory)
        {
            var builder = new ContainerBuilder();

            var assemblyTypes = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.GetInterfaces().Any()).ToArray();

            builder.RegisterTypes(assemblyTypes).AsImplementedInterfaces();

            builder.RegisterInstance(factory).As<ILoggerFactory>();
            builder.RegisterModule<LoggerModule>();

            return builder.Build();
        }
    }

    public class LoggerModule : Module
    {
        private static readonly ConcurrentDictionary<Type, object> _logCache = new ConcurrentDictionary<Type, object>();

        private interface ILoggerWrapper
        {
            object Create(ILoggerFactory factory);
        }

        protected override void AttachToComponentRegistration(
            IComponentRegistry componentRegistry,
            IComponentRegistration registration)
        {
            Ensure.Any.IsNotNull(registration, nameof(registration));

            // Handle constructor parameters.
            registration.Preparing += OnComponentPreparing;
        }

        private static object GetLogger(IComponentContext context, Type declaringType)
        {
            return _logCache.GetOrAdd(
                declaringType,
                x =>
                {
                    var factory = context.Resolve<ILoggerFactory>();
                    var loggerName = "Function." + declaringType.FullName + ".User";

                    return factory.CreateLogger(loggerName);
                });
        }

        private static void OnComponentPreparing(object sender, PreparingEventArgs e)
        {
            var t = e.Component.Activator.LimitType;

            if (t.FullName.IndexOf(nameof(KokeiroLifeLogger.Services), StringComparison.OrdinalIgnoreCase) == -1)
            {
                return;
            }

            if (t.FullName.EndsWith("[]", StringComparison.OrdinalIgnoreCase))
            {
                // Ignore IEnumerable types
                return;
            }

            e.Parameters = e.Parameters.Union(
                new[]
                {
                    new ResolvedParameter((p, i) => p.ParameterType == typeof(ILogger), (p, i) => GetLogger(i, t))
                });
        }
    }

    */
}
