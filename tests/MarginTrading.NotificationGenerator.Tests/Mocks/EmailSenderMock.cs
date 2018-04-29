using System.Threading.Tasks;
using Lykke.Service.EmailSender;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class EmailSenderMock : IEmailSender
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public Task SendAsync(EmailMessage message, EmailAddressee to)
        {
            return Task.CompletedTask;
        }
    }
}