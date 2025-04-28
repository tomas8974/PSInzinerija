using Backend.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Backend.Wrappers
{
    public class SmtpClientWrapper : ISmtpClient
    {
        public SmtpClient SmtpClient { get; set; }
        public SmtpClientWrapper(string host, int port, string username, string password, bool useSsl)
        {
            SmtpClient = new SmtpClient(host, port);
            SmtpClient.Credentials = new NetworkCredential(username, password);
            SmtpClient.EnableSsl = useSsl;
        }
        public async Task SendMailAsync(MailMessage mailMessage)
        {
            await SmtpClient.SendMailAsync(mailMessage);
        }
    }
}
