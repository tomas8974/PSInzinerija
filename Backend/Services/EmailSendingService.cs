namespace Backend.Services
{
    using System.Net.Mail;
    using System.Threading.Tasks;
    using Backend.Data.Models;
    using Backend.Interfaces;

    using Microsoft.AspNetCore.Identity;

    public class EmailSendingService : IEmailSender<User>
    {
        private readonly ISmtpClient _smtpClient;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailSendingService(
            ISmtpClient smtpClient,
            string fromEmail,
            string fromName)
        {
            _smtpClient = smtpClient;
            _fromEmail = fromEmail;
            _fromName = fromName;
        }

        public async Task SendPasswordResetCodeAsync(User user, string email, string code)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = "Password Reset Request",
                Body = $"Please use the following code to reset your password: {code}",
                IsBodyHtml = false  // Change this to `true` if you want to send HTML emails
            };

            message.To.Add(new MailAddress(email));

            try
            {
                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }

        public async Task SendPasswordResetLinkAsync(User user, string email, string resetUrl)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = "Password Reset Request",
                Body = $"Please use the following link to reset your password: {resetUrl}",
                IsBodyHtml = false
            };

            message.To.Add(new MailAddress(email));

            try
            {
                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }
        }
        public async Task SendConfirmationLinkAsync(User user, string email, string confirmationUrl)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = "Account Confirmation",
                Body = $"Please use the following link to confirm your account: {confirmationUrl}",
                IsBodyHtml = false
            };

            message.To.Add(new MailAddress(email));

            try
            {
                await _smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                throw;
            }

        }
    }

}
