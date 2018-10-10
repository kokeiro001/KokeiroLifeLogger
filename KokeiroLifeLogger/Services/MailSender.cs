using System.Net;
using System.Net.Mail;

namespace KokeiroLifeLogger.Services
{
    public interface IMailSender
    {
        void Send(string to, string subject, string body);
    }

    public class MailSender : IMailSender
    {
        private readonly string from;
        private readonly string password;

        public MailSender(string from, string password)
        {
            this.from = from;
            this.password = password;
        }

        public void Send(string to, string subject, string body)
        {
            using (MailMessage msg = new MailMessage(from, to, subject, body))
            using (SmtpClient sc = new SmtpClient())
            {
                sc.Host = "smtp.gmail.com";
                sc.Port = 587;
                sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                sc.Credentials = new NetworkCredential(from, password);
                sc.EnableSsl = true;
                sc.Send(msg);
            }
        }
    }
}
