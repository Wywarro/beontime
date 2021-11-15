using Beontime.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Beontime.Infrastructure.EmailSender
{

    public sealed class EmailService : IEmailService
    {
        private readonly EmailConfigSettings emailConfig;

        public EmailService(IOptions<EmailConfigSettings> emailConfig)
        {
            this.emailConfig = emailConfig.Value;
        }

        public async Task SendEmailAsync(string receiver, string subject, string content)
        {
            var message = new EmailMessage
            {
                Sender = emailConfig.Username,
                Reciever = receiver,
                Subject = subject,
                Content = content
            };
            var email = CreateMimeMessageFromEmailMessage(message);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(emailConfig.SmtpServer,
                emailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailConfig.Username, emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private static MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(MailboxAddress.Parse(message.Sender));
            mimeMessage.To.Add(MailboxAddress.Parse(message.Reciever));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(TextFormat.Html)
            { Text = message.Content };
            return mimeMessage;
        }
    }
}
