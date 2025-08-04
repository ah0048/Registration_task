using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using MimeKit;

namespace Backend.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSenderService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_config["Gmail:From"]));
            msg.To.Add(MailboxAddress.Parse(email));
            msg.Subject = subject;
            msg.Body = new TextPart("html") { Text = htmlMessage };

            using var smtp = new SmtpClient();
            //smtp.CheckCertificateRevocation = false;

            
            int port = int.Parse(_config["Gmail:Port"]);
            await smtp.ConnectAsync(_config["Gmail:Host"], port, SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(_config["Gmail:From"], _config["Gmail:AppPassword"]);

            await smtp.SendAsync(msg);

            await smtp.DisconnectAsync(true);
        }
    }
}
