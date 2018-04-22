using System.Threading.Tasks;
using MarginTrading.NotificationGenerator.Core.Services;
using Lykke.Service.EmailSender;

namespace MarginTrading.NotificationGenerator.Services
{
    public class EmailService : IEmailService
    {
        private readonly ITemplateGenerator _templateGenerator;
        private readonly IEmailSender _emailSender;

        public EmailService(ITemplateGenerator templateGenerator, IEmailSender emailSender)
        {
            _templateGenerator = templateGenerator;
            _emailSender = emailSender;
        }
        
        public async Task PrepareAndSendEmailAsync<T>(string email, string subj, string templateName, T model)
            where T : class
        {
            var message = _templateGenerator.Generate(templateName, model);
            
            await _emailSender.SendAsync(
                new EmailMessage
                {
                    HtmlBody = message,
                    Subject = subj
                },
                new EmailAddressee
                {
                    EmailAddress = email
                });
        }
    }
}