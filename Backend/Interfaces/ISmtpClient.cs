using System.Net.Mail;

namespace Backend.Interfaces
{
    public interface ISmtpClient
    {
        Task SendMailAsync(MailMessage mailMessage);
    }
}
