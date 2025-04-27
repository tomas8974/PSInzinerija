using Moq;
using System.Net.Mail;
using Backend.Services;
using Backend.Data.Models;
using Backend.Interfaces;

public class EmailSendingServiceTests
{
    private readonly Mock<ISmtpClient> _smtpClientMock;
    private readonly EmailSendingService _emailSendingService;
    private readonly string _fromEmail = "noreply@example.com";
    private readonly string _fromName = "Test Service";

    public EmailSendingServiceTests()
    {
        _smtpClientMock = new Mock<ISmtpClient>();
        _emailSendingService = new EmailSendingService(
            _smtpClientMock.Object,
            _fromEmail,
            _fromName
        );
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_SendsCorrectEmail()
    {
        var user = new User();
        var email = "user@example.com";
        var code = "123456";

        var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = "Password Reset Request",
            Body = $"Please use the following code to reset your password: {code}",
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(email));

        await _emailSendingService.SendPasswordResetCodeAsync(user, email, code);

        _smtpClientMock.Verify(client => client.SendMailAsync(It.Is<MailMessage>(msg =>
            msg.Subject == message.Subject &&
            msg.Body == message.Body &&
            msg.To.ToString() == message.To.ToString()
        )), Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetLinkAsync_SendsCorrectEmail()
    {
        var user = new User();
        var email = "user@example.com";
        var resetUrl = "https://example.com/reset?code=123456";

        var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = "Password Reset Request",
            Body = $"Please use the following link to reset your password: {resetUrl}",
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(email));

        await _emailSendingService.SendPasswordResetLinkAsync(user, email, resetUrl);

        _smtpClientMock.Verify(client => client.SendMailAsync(It.Is<MailMessage>(msg =>
            msg.Subject == message.Subject &&
            msg.Body == message.Body &&
            msg.To.ToString() == message.To.ToString()
        )), Times.Once);
    }

    [Fact]
    public async Task SendConfirmationLinkAsync_SendsCorrectEmail()
    {
        var user = new User();
        var email = "user@example.com";
        var confirmationUrl = "https://example.com/confirm?code=123456";

        var message = new MailMessage
        {
            From = new MailAddress(_fromEmail, _fromName),
            Subject = "Account Confirmation",
            Body = $"Please use the following link to confirm your account: {confirmationUrl}",
            IsBodyHtml = false
        };
        message.To.Add(new MailAddress(email));

        await _emailSendingService.SendConfirmationLinkAsync(user, email, confirmationUrl);

        _smtpClientMock.Verify(client => client.SendMailAsync(It.Is<MailMessage>(msg =>
            msg.Subject == message.Subject &&
            msg.Body == message.Body &&
            msg.To.ToString() == message.To.ToString()
        )), Times.Once);
    }

    [Fact]
    public async Task SendPasswordResetCodeAsync_WhenSmtpClientThrowsException_ThrowsException()
    {
        var user = new User();
        var email = "user@example.com";
        var code = "123456";

        _smtpClientMock.Setup(client => client.SendMailAsync(It.IsAny<MailMessage>()))
            .ThrowsAsync(new Exception("SMTP error"));

        await Assert.ThrowsAsync<Exception>(async () =>
            await _emailSendingService.SendPasswordResetCodeAsync(user, email, code)
        );
    }
}
