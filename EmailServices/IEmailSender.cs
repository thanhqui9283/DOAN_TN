using SendGrid;
using System.Threading.Tasks;

namespace EmailServices
{
    public interface IEmailSender
    {
        Task<Response> SendEmailAsync(Message message);
    }
}
