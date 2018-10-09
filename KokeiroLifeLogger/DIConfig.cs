using Autofac;
using AzureFunctions.Autofac.Configuration;
using KokeiroLifeLogger.Services;
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

                //Registration by autofac module
                //builder.RegisterModule(new TestModule());
                //Named Instances are supported
                //builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                //builder.RegisterType<Thing2>().Named<IThing>("OptionB");
            }, functionName);
        }
    }
}
