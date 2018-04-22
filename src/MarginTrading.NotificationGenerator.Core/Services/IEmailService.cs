using System.Threading.Tasks;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface IEmailService
    {
        Task PrepareAndSendEmailAsync<T>(string email, string subj, string templateName, T model) where T : class;
    }
}