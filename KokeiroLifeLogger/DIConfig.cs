using Autofac;
using AzureFunctions.Autofac.Configuration;
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
                //Implicity registration
                //builder.RegisterType<Sample>().As<ISample>();
                builder.RegisterType<BlogPvStringLoader>().As<IBlogPvStringLoader>();

                builder.Register<LifeLogCrawler>(c =>
                {
                    return new LifeLogCrawler(c.Resolve<IBlogPvStringLoader>());
                })
                .As<ILifeLogCrawler>();

                builder.Register<MailSender>(c =>
                {
                    var from = ConfigurationManager.AppSettings["MixiPostMail"];
                    var password = ConfigurationManager.AppSettings["MixiPostMailPassword"];
                    return new MailSender(from, password);
                })
                .As<ILifeLogCrawler>();


                builder.Register<NicoNicoMyListObserveService>(c =>
                {
                    var connectionString = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
                    var storageAccount = CloudStorageAccount.Parse(connectionString);

                    return new NicoNicoMyListObserveService(storageAccount);
                })
                .As<INicoNicoMyListObserveService>();


                //builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                //builder.RegisterType<Thing2>().Named<IThing>("OptionB");

                //Registration by autofac module
                //builder.RegisterModule(new TestModule());
                //Named Instances are supported
                //builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                //builder.RegisterType<Thing2>().Named<IThing>("OptionB");
            }, functionName);
        }
    }
}
