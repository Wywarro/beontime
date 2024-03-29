﻿namespace Beontime.Infrastructure.EmailSender
{
    public sealed class EmailConfigSettings
    {
        public const string SectionName = "EmailConfig";

        public string SmtpServer { get; set; } = "";
        public int Port { get; set; }
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
