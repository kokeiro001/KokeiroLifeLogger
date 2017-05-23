using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using System.Threading.Tasks;
using System.Text;

namespace KokeiroLifeLogger
{
    // 1日の区切りをAM5:00とした日記を自動投稿する.
    // 時差注意！

    public static class PostDiary2Mixics
    {
        [FunctionName("PostDiary2Mixics")]
        public static async Task Run([TimerTrigger("0 0 20 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var from = ConfigurationManager.AppSettings["MixiPostMail"];
            var to = ConfigurationManager.AppSettings["MixiPostMailTo"];

            var crawler = new LifeLogCrawler();
            var lifeLog = await crawler.CrawlAsync();
            var fromPassword = ConfigurationManager.AppSettings["MixiPostMailPassword"];

            var isLocal = CloudConfigurationManager.GetSetting("IsLocal");
            if (isLocal == "true")
            {
                log.Info($"local running!!! skip SendMail. Title={lifeLog.Title}, Body={lifeLog.Body}");
                return;
            }
            SendMail(from, to, lifeLog.Title, lifeLog.Body, fromPassword);
        }

        private static void SendMail(string from, string to, string subject, string body, string fromPassword)
        {
            using (MailMessage msg = new MailMessage(from, to, subject, body))
            using (SmtpClient sc = new SmtpClient())
            {
                sc.Host = "smtp.gmail.com";
                sc.Port = 587;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Credentials = new NetworkCredential(from, fromPassword);
                sc.EnableSsl = true;
                sc.Send(msg);
            }
        }
    }
}