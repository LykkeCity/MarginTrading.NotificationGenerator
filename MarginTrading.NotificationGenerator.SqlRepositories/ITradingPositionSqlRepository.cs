using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.NotificationGenerator.Core.Domain;

namespace MarginTrading.NotificationGenerator.SqlRepositories
{
    public interface ITradingPositionSqlRepository
    {
        Task<IEnumerable<TradingPosition>> GetClosedFromPeriod(DateTime from, DateTime to);
    }
}