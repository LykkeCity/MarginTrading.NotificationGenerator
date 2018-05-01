using System.Collections.Generic;
using System.Threading.Tasks;
using MarginTrading.Backend.Contracts;
using MarginTrading.Backend.Contracts.AssetPairSettings;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class AssetPairsReadingMock : IAssetPairsReadingApi
    {
        public Task<List<AssetPairContract>> List(string legalEntity = null, MatchingEngineModeContract? matchingEngineMode = null)
        {
            return Task.FromResult(new List<AssetPairContract>
            {
                new AssetPairContract
                {
                    Id = "BTCUSDcy",
                    Name = "BTCUSD"
                }
            });
        }

        public async Task<AssetPairContract> Get(string assetPairId)
        {
            throw new System.NotImplementedException();
        }
    }
}