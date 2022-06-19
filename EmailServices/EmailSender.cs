using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace EmailServices
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        private string ApiKey;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task<Response> SendEmailAsync(Message message)
        {
            ApiKey = _emailConfig.SendGridAPIKey;
            var client = new SendGridClient(ApiKey);
            var from = message.FromMail;
            var subject = message.Subject;
            var to = message.To;
            var htmlContent = message.Content;
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlContent);
            var response = await client.SendEmailAsync(msg);
            return response;
        }
    }
}
