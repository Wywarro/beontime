﻿namespace Beontime.Infrastructure.EmailSender
{
    public sealed class EmailSenderMetadata
    {
        public string SmtpServer { get; set; } = "";
        public int Port { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
