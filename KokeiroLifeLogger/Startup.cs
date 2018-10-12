using KokeiroLifeLogger.Repository;
using KokeiroLifeLogger.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace KokeiroLifeLogger
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;

        public Startup(ILogger<Startup> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<ClassThatNeedsInjection>();

            services.AddScoped<IConfigProvider>(service =>
            {
                var executingPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var config = new ConfigurationBuilder()
                    .SetBasePath(Path.GetDirectoryName(executingPath))
                    .AddJsonFile("local.settings.json", true, true)
                    .AddEnvironmentVariables()
                    .Build();

                return new ConfigProvider(config);
            });

            services.AddScoped<CloudStorageAccountProvider>(service =>
            {
                var config = service.GetService<IConfigProvider>().GetConfig();
                var connectionString = config["AzureWebJobsStorage"];
                var cloudStorageAccount = CloudStorageAccount.Parse(connectionString);

                return new CloudStorageAccountProvider(cloudStorageAccount);
            });

            services.AddScoped<IIFTTTRepository, IFTTTRepository>();
            services.AddScoped<ILocationEnteredOrExitedRepository, LocationEnteredOrExitedRepository>();
            services.AddScoped<IWeightMeasurementRepository, WeightMeasurementRepository>();

            services.AddScoped<BlogPvStringLoader>(service =>
            {
                var config = service.GetService<IConfigProvider>().GetConfig();

                return new BlogPvStringLoader(
                    config["HatebuViewId"],
                    config["QiitaViewId"],
                    service.GetService<IGoogleAnalyticsReader>()
                );
            });

            services.AddScoped<IGitHubContributionsReader, GitHubContributionsReader>();
            services.AddScoped<IGoogleAnalyticsReader, GoogleAnalyticsReader>();
            services.AddScoped<ILifeLogCrawler, LifeLogCrawler>();
            services.AddScoped<IIFTTTService, IFTTTService>();
            services.AddScoped<ILifeLogCrawler, LifeLogCrawler>();
            services.AddScoped<ILocationEnteredOrExitedService, LocationEnteredOrExitedService>();
            services.AddScoped<INicoNicoMyListObserveService, NicoNicoMyListObserveService>();
            services.AddScoped<IWeightMeasurementService, WeightMeasurementService>();

            services.AddScoped<MailSender>(service =>
            {
                var config = service.GetService<IConfigProvider>().GetConfig();
                var from = config["MixiPostMail"];
                var password = config["MixiPostMailPassword"];
                return new MailSender(from, password);
            });
        }

        public void Configure(IConfigurationBuilder app)
        {
            var executingAssembly = new FileInfo(Assembly.GetExecutingAssembly().Location);
            _logger.LogInformation($"Using \"{executingAssembly.Directory.FullName}\" as base path to load configuration files.");
            app
                .SetBasePath(executingAssembly.Directory.FullName)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
