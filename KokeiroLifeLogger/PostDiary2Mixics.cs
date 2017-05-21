using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace KokeiroLifeLogger
{
    // 1日の区切りをAM5:00とした日記を自動投稿する

    public static class PostDiary2Mixics
    {
        [FunctionName("TimerTriggerCSharp")]
        //public static void Run([TimerTrigger("*/5 * * * * *")]TimerInfo myTimer, TraceWriter log)
        public static void Run([TimerTrigger("0 0 5 * * *")]TimerInfo myTimer, TraceWriter log)
        {
            log.Info($"C# Timer trigger function executed at: {DateTime.Now}");
            var from = ConfigurationManager.AppSettings["MixiPostMail"];
            var to = ConfigurationManager.AppSettings["MixiPostMailTo"];
            var subject = GetTitle();
            var body = GetBody();
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
            return DateTime.Now.Date.AddDays(-1).ToString("yyyymmdd") + "のライフログ";
        }

        private static string GetBody()
        {
            var now = DateTime.Now.Date;

            var fromDate = now.Date.AddDays(-1).AddHours(5);
            var toDate = fromDate.AddDays(1);

            // TODO: 必要な情報を適当なストレージから引っ張ってきて整形する。
            // Model用意したほうが良さそうね。

            return "hogehoge";
        }
    }
}