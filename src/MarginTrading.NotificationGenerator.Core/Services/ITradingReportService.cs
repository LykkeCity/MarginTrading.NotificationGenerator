using System.Threading.Tasks;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface ITradingReportService
    {
        Task PerformReporting();
        Task PerformReportingMonthly();
    }
}