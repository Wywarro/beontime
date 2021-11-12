using System.Threading.Tasks;

namespace Beontime.Application.Common.Interfaces
{

    public interface IEmailService
    {
        Task SendEmailAsync(string receiver, string subject, string content);
    }
}
