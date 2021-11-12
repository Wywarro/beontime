using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System.Threading.Tasks;

namespace Beontime.Infrastructure.EmailSender
{

    public sealed class EmailService
    {
        private readonly EmailSenderMetadata senderMetadata;

        public EmailService(IOptions<EmailSenderMetadata> emailSenderMetadata)
        {
            senderMetadata = emailSenderMetadata.Value;
        }

        public async Task SendEmailAsync(string receiver, string subject, string content)
        {
            var message = new EmailMessage
            {
                Sender = senderMetadata.Username,
                Reciever = receiver,
                Subject = subject,
                Content = content
            };
            var email = CreateMimeMessageFromEmailMessage(message);

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(senderMetadata.SmtpServer,
                senderMetadata.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderMetadata.Username, senderMetadata.Password);
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
