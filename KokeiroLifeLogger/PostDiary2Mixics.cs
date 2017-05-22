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
        [FunctionName("TimerTriggerCSharp")]
        //public static async Task Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        public static async Task Run([TimerTrigger("0 0 20 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var from = ConfigurationManager.AppSettings["MixiPostMail"];
            var to = ConfigurationManager.AppSettings["MixiPostMailTo"];
            var subject = GetTitle();
            var body = await GetBodyAsync();
            var fromPassword = ConfigurationManager.AppSettings["MixiPostMailPassword"];

            SendMail(from, to, subject, body, fromPassword);
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

        private static string GetTitle()
        {
            return DateTime.Now.Date.ToString("yyyymmdd") + "のライフログ";
        }

        private static async Task<string> GetBodyAsync()
        {
            var now = DateTime.Now;

            var from = now.Date.AddDays(-1);
            var to = now;

            var sb = new StringBuilder();

            sb.AppendLine("※自動ポスト※");
            sb.AppendLine();
            sb.AppendLine();

            sb.AppendLine(await IFTTTHttpTrigger.GetData(from, to));

            return sb.ToString();
        }
    }
}