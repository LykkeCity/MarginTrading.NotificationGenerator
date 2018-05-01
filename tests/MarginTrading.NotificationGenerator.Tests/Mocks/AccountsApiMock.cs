using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarginTrading.Backend.Contracts;
using MarginTrading.Backend.Contracts.Account;
using Moq;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class AccountsApiMock : IAccountsApi
    {
        public static IEnumerable<DataReaderAccountBackendContract> GetAccounts()
        {
            yield return new DataReaderAccountBackendContract
            {
                Id = "a1",
                ClientId = "c1",
                TradingConditionId = "tc1",
                BaseAssetId = "BTC",
                Balance = 100,
                WithdrawTransferLimit = 100,
                IsLive = true,
                LegalEntity = "LYKKECY",
            };
            yield return new DataReaderAccountBackendContract
            {
                Id = "a2",
                ClientId = "c2",
                TradingConditionId = "tc2",
                BaseAssetId = "USD",
                Balance = 2000,
                WithdrawTransferLimit = 1000,
                IsLive = true,
                LegalEntity = "LYKKEUK",
            };
        }
        
        public Task<IEnumerable<DataReaderAccountBackendContract>> GetAllAccounts()
        {
            return Task.FromResult(GetAccounts());
        }

        public Task<IEnumerable<DataReaderAccountStatsBackendContract>> GetAllAccountStats()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<DataReaderAccountBackendContract>> GetAccountsByClientId(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DataReaderAccountBackendContract> GetAccountById(string id)
        {
            throw new System.NotImplementedException();
        }
    }
}