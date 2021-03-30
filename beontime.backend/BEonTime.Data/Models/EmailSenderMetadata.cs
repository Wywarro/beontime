using System;
using System.Collections.Generic;
using System.Text;

namespace BEonTime.Data.Models
{
    public class EmailSenderMetadata
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
