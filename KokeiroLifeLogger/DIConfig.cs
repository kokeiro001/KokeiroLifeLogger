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
