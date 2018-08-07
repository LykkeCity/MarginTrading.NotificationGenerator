using System.Threading.Tasks;
using MarginTrading.NotificationGenerator.Core.Domain;

namespace MarginTrading.NotificationGenerator.Core.Services
{
    public interface ITradingReportService
    {
        Task PerformReporting(OvernightSwapReportType reportType);
    }
}