using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.Backend.Contracts;
using MarginTrading.Backend.Contracts.AccountHistory;
using MarginTrading.Backend.Contracts.AssetPairSettings;
using MarginTrading.Backend.Contracts.TradeMonitoring;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class AccountHistoryApiMock : IAccountHistoryApi
    {
        public static IEnumerable<AccountHistoryContract> GetAccountHistory()
        {
            yield return new AccountHistoryContract
            {
                Id = "tr1",
                Date = DateTime.Parse("2018-04-26 12:00:00.000"),
                AccountId = "a1",
                ClientId = "c1",
                Amount = 0.5M,
                Balance = 100.5M,
                Comment = "Suspicious transfer",
                Type = AccountHistoryTypeContract.Deposit,
                OrderId = Guid.NewGuid().ToString(),
                LegalEntity = "LYKKECY",
            };
            yield return new AccountHistoryContract
            {
                Id = "tr2",
                Date = DateTime.Parse("2018-04-26 13:00:00.000"),
                AccountId = "a1",
                ClientId = "c1",
                Amount = -1M,
                Balance = 100,
                Comment = "Withdrawal",
                Type = AccountHistoryTypeContract.Withdraw,
                OrderId = Guid.NewGuid().ToString(),
                LegalEntity = "LYKKECY",
            };
            yield return new AccountHistoryContract
            {
                Id = "tr3",
                Date = DateTime.Parse("2018-04-29 21:50:50.000"),
                AccountId = "a2",
                ClientId = "c2",
                Amount = 1000M,
                Balance = 2000,
                Comment = "Order closed",
                Type = AccountHistoryTypeContract.OrderClosed,
                OrderId = Guid.NewGuid().ToString(),
                LegalEntity = "LYKKECY",
            };
        }

        public static IEnumerable<OrderHistoryContract> GetOpenPositions()
        {
            yield return new OrderHistoryContract
            {
                Id = "o1",
                AccountId = "a1",
                Instrument = "BTCUSD",
                Type = OrderDirectionContract.Sell,
                Status = OrderStatusContract.Active,
                OpenDate = DateTime.Parse("2018-04-28 20:00:00.000"),
                OpenPrice = 1000,
                Volume = -0.3M,
                TakeProfit = 1100,
                StopLoss = 900,
                Pnl = -0.04M,
                MatchingEngineMode = MatchingEngineModeContract.Stp,
            };
        }

        public static IEnumerable<OrderHistoryContract> GetPositionHistory()
        {
            yield return new OrderHistoryContract
            {
                Id = "h1",
                AccountId = "a2",
                Instrument = "EURCHF",
                Type = OrderDirectionContract.Sell,
                Status = OrderStatusContract.Closed,
                OpenDate = DateTime.Parse("2018-04-27 10:00:00.000"),
                OpenPrice = 1.1M,
                CloseDate = DateTime.Parse("2018-04-27 16:00:00.000"),
                ClosePrice = 1.14M,
                Volume = -100M,
                TakeProfit = 1.2M,
                StopLoss = 1,
                Pnl = -4M,
                MatchingEngineMode = MatchingEngineModeContract.Stp,
            };
        }
        
        public Task<AccountHistoryResponse> ByTypes(AccountHistoryRequest request)
        {
            return Task.FromResult(new AccountHistoryResponse
            { 
                Account = GetAccountHistory().ToArray(),
                OpenPositions = GetOpenPositions().ToArray(),
                PositionsHistory = GetPositionHistory().ToArray(),
            });
        }

        public Task<Dictionary<string, AccountHistoryContract[]>> ByAccounts(string accountId = null, DateTime? @from = null, DateTime? to = null)
        {
            throw new NotImplementedException();
        }

        public Task<AccountNewHistoryResponse> Timeline(AccountHistoryRequest request)
        {
            throw new NotImplementedException();
        }
    }
}