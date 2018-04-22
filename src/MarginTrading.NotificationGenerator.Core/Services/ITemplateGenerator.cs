#pragma warning disable 1591

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface ITemplateGenerator
    {
        string Generate<T>(string templateName, T model);
    }
}
