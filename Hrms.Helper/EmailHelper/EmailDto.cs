using System;
using System.Collections.Generic;

namespace Hrms.Helper.EmailHelper
{
    public class EmailDto
    {
        public string SmtpServer { get; set; }
        public string SmtpUsername { get; set; }
        public string Port { get; set; }
        public string SmtpPassword { get; set; }
        public EmailAddress From { get; set; }
        public List<EmailMessage> Messages { get; set; }
    }

    public class EmailMessage
    {
        public string Subject { get; set; }
        public string Content { get; set; }
        public List<EmailAddress> To { get; set; }
        public List<EmailAddress> Cc { get; set; }
        public List<EmailAddress> Bcc { get; set; } 
    }

    public class EmailAddress
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
