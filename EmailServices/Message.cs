using MimeKit;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;

namespace EmailServices
{
    public class Message
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailAddress FromMail { get; set; }
        public EmailAddress To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }

        public Message(string to, string subject, string content, EmailConfiguration configuration)
        {
            _emailConfig = configuration;
            FromMail = new EmailAddress(_emailConfig.Mail);
            FromMail.Name = _emailConfig.From;
            //FromMail.Name = _emailConfig.From;
            To = new EmailAddress(to);
            Subject = subject;
            Content = content;
        }
    }
}
