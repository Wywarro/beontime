using BEonTime.Data.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit.Security;
using System.Threading.Tasks;
using MimeKit.Text;

namespace BEonTime.Services.EmailSender
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string receiver, string subject, string content);
    }

    public class EmailSender : IEmailSender
    {
        private readonly EmailSenderMetadata senderMetadata;

        public EmailSender(IOptions<EmailSenderMetadata> emailSenderMetadata)
        {
            senderMetadata = emailSenderMetadata.Value;
        }

        public async Task SendEmailAsync(string receiver, string subject, string content)
        {
            EmailMessage message = new EmailMessage
            {
                Sender = senderMetadata.Username,
                Reciever = receiver,
                Subject = subject,
                Content = content
            };
            var email = CreateMimeMessageFromEmailMessage(message);

            // send email
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(senderMetadata.SmtpServer, 
                senderMetadata.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(senderMetadata.Username, senderMetadata.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
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
