using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.Backend.Contracts;
using MarginTrading.Backend.Contracts.AssetPairSettings;
using MarginTrading.Backend.Contracts.TradeMonitoring;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class TradeMonitoringReadingApiMock : ITradeMonitoringReadingApi
    {
        public static IEnumerable<OrderContract> GetPendings()
        {
            yield return new OrderContract
            {
                Id = "p1",
                ClientId = "c1",
                AccountId = "a1",
                AccountAssetId = "BTC",
                Instrument = "BTCUSD",
                Type = OrderDirectionContract.Buy,
                Status = OrderStatusContract.WaitingForExecution,
                ExpectedOpenPrice = 1000,
                CreateDate = DateTime.UtcNow,
                Volume = 1.1M,
                TakeProfit = 1100,
                StopLoss = 900,
                PnL = 0.12M,
                LegalEntity = "LYKKECY",
                MatchingEngineMode = MatchingEngineModeContract.Stp,
            };
            yield return new OrderContract
            {
                Id = "p2",
                ClientId = "c1",
                AccountId = "a1",
                AccountAssetId = "BTC",
                Instrument = "EURCHF",
                Type = OrderDirectionContract.Sell,
                Status = OrderStatusContract.WaitingForExecution,
                ExpectedOpenPrice = 1.2M,
                CreateDate = DateTime.UtcNow,
                Volume = 120M,
                TakeProfit = 1.3M,
                StopLoss = 1.1M,
                PnL = 10.1M,
                LegalEntity = "LYKKECY",
                MatchingEngineMode = MatchingEngineModeContract.Stp,
            };
        }
        
        public Task<List<OrderContract>> PendingOrders()
        {
            return Task.FromResult(GetPendings().ToList());
        }
        
        public Task<List<SummaryAssetContract>> AssetSummaryList()
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> OpenPositions()
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> OpenPositionsByVolume(decimal volume)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> OpenPositionsByDate(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> OpenPositionsByClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> PendingOrdersByVolume(decimal volume)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> PendingOrdersByDate(DateTime @from, DateTime to)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderContract>> PendingOrdersByClient(string clientId)
        {
            throw new NotImplementedException();
        }

        public Task<List<OrderBookContract>> OrderBooksByInstrument(string instrument)
        {
            throw new NotImplementedException();
        }
    }
}