using Autofac;
using AzureFunctions.Autofac.Configuration;
using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KokeiroLifeLogger
{
    class DIConfig
    {
        public DIConfig(string functionName)
        {
            DependencyInjection.Initialize(builder =>
            {
                builder.Register<CloudStorageAccountProvider>(c =>
                {
                    var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
                    var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

                    return new CloudStorageAccountProvider(cloudStorageAccount);
                })
                .As<ICloudStorageAccountProvider>();

                builder.RegisterType<IFTTTRepository>().As<IIFTTTRepository>();
                builder.RegisterType<LocationEnteredOrExitedRepository>().As<ILocationEnteredOrExitedRepository>();
                builder.RegisterType<WeightMeasurementRepository>().As<IWeightMeasurementRepository>();


                builder.Register<BlogPvStringLoader>(c =>
                {
                    return new BlogPvStringLoader(
                        CloudConfigurationManager.GetSetting("HatebuViewId"),
                        CloudConfigurationManager.GetSetting("QiitaViewId"),
                        c.Resolve<IGoogleAnalyticsReader>()
                    );
                })
                .As<IBlogPvStringLoader>();

                builder.RegisterType<GitHubContributionsReader>().As<IGitHubContributionsReader>();
                builder.RegisterType<GoogleAnalyticsReader>().As<IGoogleAnalyticsReader>();
                builder.RegisterType<LifeLogCrawler>().As<ILifeLogCrawler>();
                builder.RegisterType<IFTTTService>().As<IIFTTTService>();
                builder.RegisterType<LifeLogCrawler>().As<ILifeLogCrawler>();
                builder.RegisterType<LocationEnteredOrExitedService>().As<ILocationEnteredOrExitedService>();
                builder.RegisterType<NicoNicoMyListObserveService>().As<INicoNicoMyListObserveService>();
                builder.RegisterType<WeightMeasurementService>().As<>(IWeightMeasurementService);

                builder.Register<MailSender>(c =>
                {
                    var from = ConfigurationManager.AppSettings["MixiPostMail"];
                    var password = ConfigurationManager.AppSettings["MixiPostMailPassword"];
                    return new MailSender(from, password);
                })
                .As<IMailSender>();

            }, functionName);
        }
    }
}
