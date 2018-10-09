using Autofac;
using AzureFunctions.Autofac.Configuration;
using KokeiroLifeLogger.Services;
using System;
using System.Collections.Generic;
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

                //Explicit registration
                //builder.Register<ConnectionStringProvider>(c =>
                //{
                //    return new ConnectionStringProvider("ultra connection string");
                //})
                //.As<IConnectionStringProvider>();

                //Registration by autofac module
                //builder.RegisterModule(new TestModule());
                //Named Instances are supported
                //builder.RegisterType<Thing1>().Named<IThing>("OptionA");
                //builder.RegisterType<Thing2>().Named<IThing>("OptionB");
            }, functionName);
        }
    }
}
