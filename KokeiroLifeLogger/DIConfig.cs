﻿using Autofac;
using AzureFunctions.Autofac.Configuration;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using Microsoft.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading;

namespace KokeiroLifeLogger
{
    class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.Register<ConfigProvider>(c =>
                {
                    var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                    var config = new ConfigurationBuilder()
                        .SetBasePath(Path.GetDirectoryName(executingPath))
                        .AddJsonFile("local.settings.json", true, true)
                        .AddEnvironmentVariables()
                        .Build();

                    return new ConfigProvider(config);
                })
                .As<IConfigProvider>();

                builder.Register<CloudStorageAccountProvider>(c =>
                {
                    var config = c.Resolve<IConfigProvider>().GetConfig();
                    var connectionString = config["AzureWebJobsStorage"];
                    var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

                    return new CloudStorageAccountProvider(cloudStorageAccount);
                })
                .As<ICloudStorageAccountProvider>();

                builder.RegisterType<IFTTTRepository>().As<IIFTTTRepository>();
                builder.RegisterType<LocationEnteredOrExitedRepository>().As<ILocationEnteredOrExitedRepository>();
                builder.RegisterType<WeightMeasurementRepository>().As<IWeightMeasurementRepository>();
                builder.RegisterType<WithingsSleepRepository>().As<IWithingsSleepRepository>();

                builder.Register<BlogAnalytcsService>(c =>
                {
                    var config = c.Resolve<IConfigProvider>().GetConfig();

                    return new BlogAnalytcsService(
                        config["HatebuViewId"],
                        config["QiitaViewId"],
                        c.Resolve<IGoogleAnalyticsReader>()
                    );
                })
                .As<IBlogAnalytcsService>();

                builder.RegisterType<GitHubService>().As<IGitHubService>();
                builder.RegisterType<GoogleAnalyticsReader>().As<IGoogleAnalyticsReader>();
                builder.RegisterType<LifeLogCrawler>().As<ILifeLogCrawler>();
                builder.RegisterType<IFTTTService>().As<IIFTTTService>();
                builder.RegisterType<LifeLogCrawler>().As<ILifeLogCrawler>();
                builder.RegisterType<LocationEnteredOrExitedService>().As<ILocationEnteredOrExitedService>();
                builder.RegisterType<NicoNicoMyListObserveService>().As<INicoNicoMyListObserveService>();
                builder.RegisterType<WeightMeasurementService>().As<IWeightMeasurementService>();
                builder.RegisterType<WithingsSleepService>().As<IWithingsSleepService>();

                builder.Register<MailSender>(c =>
                {
                    var config = c.Resolve<IConfigProvider>().GetConfig();
                    var from = config["MixiPostMail"];
                    var password = config["MixiPostMailPassword"];
                    return new MailSender(from, password);
                })
                .As<IMailSender>();

            }, functionName);
        }
    }

    public interface IConfigProvider
    {
        IConfigurationRoot GetConfig();
    }

    public class ConfigProvider : IConfigProvider
    {
        private readonly IConfigurationRoot config;

        public ConfigProvider(IConfigurationRoot config)
        {
            this.config = config;
        }

        public IConfigurationRoot GetConfig()
        {
            return config;
        }
    }
}
