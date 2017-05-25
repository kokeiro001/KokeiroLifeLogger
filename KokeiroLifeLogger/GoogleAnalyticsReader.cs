using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace KokeiroLifeLogger
{
    class GoogleAnalyticsReader
    {
        static readonly string AppName = @"KokeiroLifeLogger";
        static readonly string KeyFileContainerName = @"etc";
        static readonly string KeyFileName = @"blog_analytics_api_key.p12";

        string GoogleServiceAccount { get => CloudConfigurationManager.GetSetting("BlogPvServicerAccount"); }

        public async Task<int> ReadPv(string viewId, DateTime date)
        {
            var service = await CreateServiceAsync();

            var dateStr = date.ToString("yyyy-MM-dd");
            var data = await service.Data.Ga.Get("ga:" + viewId, dateStr, dateStr, "ga:pageviews").ExecuteAsync();
            return int.Parse(data.Rows.First().First());
        }

        private async Task<AnalyticsService> CreateServiceAsync()
        {
            // Azure Web サイトで動かす場合には WEBSITE_LOAD_USER_PROFILE = 1 必須
            var certificate = new X509Certificate2(await LoadApiKeyFileAsync(), "notasecret", X509KeyStorageFlags.Exportable);

            var credential = new ServiceAccountCredential(new ServiceAccountCredential.Initializer(GoogleServiceAccount)
            {
                Scopes = new[] { AnalyticsService.Scope.Analytics, AnalyticsService.Scope.AnalyticsReadonly }
            }
            .FromCertificate(certificate));

            var service = new AnalyticsService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = AppName,
            });
            return service;
        }

        async Task<byte[]> LoadApiKeyFileAsync()
        {
            // TODO: あちこちでCloudStorageAccount作るから、Utilityに移す
            var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("AzureWebJobsStorage"));

            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(KeyFileContainerName);

            var blob = await container.GetBlobReferenceFromServerAsync(KeyFileName);
            var content = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(content, 0);

            return content;

        }
    }
}
