using Backend.Helpers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Backend.Services.Implementation
{
    public class EmailSenderService : IEmailSender
    {
        private readonly EmailSettings emailSettings;

        public EmailSenderService(IOptions<EmailSettings> options)
        {
            emailSettings = options.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(emailSettings.From));
            msg.To.Add(MailboxAddress.Parse(email));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlMessage };

            using var smtp = new SmtpClient();
            //smtp.CheckCertificateRevocation = false;

            
            await smtp.ConnectAsync(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(emailSettings.From, emailSettings.AppPassword);

            await smtp.SendAsync(msg);

            await smtp.DisconnectAsync(true);
        }
    }
}
