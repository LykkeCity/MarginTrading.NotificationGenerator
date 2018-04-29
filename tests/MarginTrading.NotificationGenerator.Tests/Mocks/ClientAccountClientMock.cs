using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.ClientAccount.Client;
using Lykke.Service.ClientAccount.Client.AutorestClient.Models;
using Lykke.Service.ClientAccount.Client.Models;

namespace MarginTrading.NotificationGenerator.Tests.Mocks
{
    public class ClientAccountClientMock : IClientAccountClient
    {
        public static IEnumerable<ClientAccountInformationModel> GetClientAccounts()
        {
            yield return new ClientAccountInformationModel
            {
                Id = "a1",
                Email = "fakemail1@fake.com"
            };
            yield return new ClientAccountInformationModel
            {
                Id = "a2",
                Email = "fakemail2@fake.com"
            };
        }
        
        public Task<IEnumerable<ClientAccountInformationModel>> GetClientsByIdsAsync(string[] ids)
        {
            return Task.FromResult(GetClientAccounts());
        }
        
        public Task<AppUsageSettingsModel> GetAppUsageAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<AssetPairsInvertedSettingsModel> GetAssetPairsInvertedAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<LastBaseAssetsIosClientModel> GetLastBaseAssetsIosAsync(string clientid)
        {
            throw new System.NotImplementedException();
        }

        public Task<LastBaseAssetsOtherClientModel> GetLastBaseAssetsOtherAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<RefundAddressSettingsModel> GetRefundAddressAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<PushNotificationsSettingsModel> GetPushNotificationAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MyLykkeSettingsModel> GetMyLykkeAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<BackupSettingsModel> GetBackupAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<SmsSettingsModel> GetSmsAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<HashedPwdSettingsModel> GetHashedPwdAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<CashOutBlockSettingsModel> GetCashOutBlockAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<DepositBlockSettingsModel> GetDepositBlockAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<MarginEnabledSettingsModel> GetMarginEnabledAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IsUsaUserSettingsModel> GetIsUsaUserAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IsOffchainUserSettingsModel> GetIsOffchainUserAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<NeedReinitSettingsModel> GetNeedReinitAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IsLimitOrdersAvailableClientModel> GetIsLimitOrdersAvailableAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<BaseAssetClientModel> GetBaseAssetAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<FeaturesSettingsModel> GetFeaturesAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPinAsync(string clientId, string pin)
        {
            throw new System.NotImplementedException();
        }

        public Task SetAppUsageAsync(string clientId, string period)
        {
            throw new System.NotImplementedException();
        }

        public Task SetAssetPairsInvertedAsync(string clientId, string[] invertedAssetIds)
        {
            throw new System.NotImplementedException();
        }

        public Task SetLastBaseAssetsIosAsync(string clientId, string[] baseAssets)
        {
            throw new System.NotImplementedException();
        }

        public Task SetLastBaseAssetsOtherAsync(string clientId, string[] baseAssets)
        {
            throw new System.NotImplementedException();
        }

        public Task SetRefundAddressAsync(string clientId, string address, int validDays, bool sendAutomatically)
        {
            throw new System.NotImplementedException();
        }

        public Task SetPushNotificationAsync(string clientId, bool enabled)
        {
            throw new System.NotImplementedException();
        }

        public Task SetMyLykkeAsync(string clienId, bool enabled)
        {
            throw new System.NotImplementedException();
        }

        public Task SetBackupAsync(string clientId, bool backupDone)
        {
            throw new System.NotImplementedException();
        }

        public Task SetSmsAsync(string clientId, bool useAlternativeProvider)
        {
            throw new System.NotImplementedException();
        }

        public Task SetHashedAsync(string clientId, bool isPwdHashed)
        {
            throw new System.NotImplementedException();
        }

        public Task SetCashOutBlockAsync(string clientId, bool cashOutBlocked, bool tradesBlocked)
        {
            throw new System.NotImplementedException();
        }

        public Task SetDepositBlockAsync(string clientId, bool depositViaCreditCardBlocked)
        {
            throw new System.NotImplementedException();
        }

        public Task SetMarginEnabledAsync(string clientId, bool enabled, bool enabledLive, bool termsOfUseAgreed)
        {
            throw new System.NotImplementedException();
        }

        public Task SetIsUsaUserAsync(string clientId, bool isUSA)
        {
            throw new System.NotImplementedException();
        }

        public Task SetIsOffchainUserAsync(string clienId, bool isOffchain)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNeedReinitAsync(string clientId, bool needReinit)
        {
            throw new System.NotImplementedException();
        }

        public Task SetIsLimitOrdersAvailableAsync(string clientId, bool available)
        {
            throw new System.NotImplementedException();
        }

        public Task SetBaseAssetAsync(string clientId, string baseAssetId)
        {
            throw new System.NotImplementedException();
        }

        public Task SetFeaturesAsync(string clientId, bool affiliateEnabled)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAppUsageAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAssetPairsInvertedAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteLastBaseAssetsIosAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteLastBaseAssetsOtherAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteRefundAddresAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeletePushNotificationAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteMyLykkeAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteBackupAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteSmsAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteHashedAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCashOutBlockAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteDepositBlockAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteMarginEnabledAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteIsUsaUserAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteIsOffchainUserAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteNeedReinitAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteIsLimitOrdersAvailableAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteBaseAssetAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteFeaturesAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<AccountExistsModel> IsTraderWithEmailExistsAsync(string email, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAccountAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task ChangeEmailAsync(string id, string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool?> IsTrustedAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientAccountInformationModel> GetClientByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<string>> GetClientsByPhoneAsync(string phoneNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsPasswordCorrectAsync(string clientId, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientModel> GetByIdAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<ClientAccountInformationModel>> GetClientsByEmailAsync(string email)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientAccountInformationModel> GetClientByEmailAndPartnerIdAsync(string email, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientAccountInformationModel> GetClientByPhoneAndPartnerIdAsync(string phoneNumber, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientAccountInformationModel> AuthenticateAsync(string email, string password, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task ChangeClientPasswordAsync(string clientId, string pwdHash)
        {
            throw new System.NotImplementedException();
        }

        public Task ChangeClientPhoneNumberAsync(string clientId, string phoneNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task InsertIndexedByPhoneAsync(string clientId, string phoneNumber, string previousPhoneNumber)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool?> IsEmailVerifiedAsync(string email, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<PartnerAccountPolicyModel> GetPartnerAccountPolicyAsync(string partnerPublicId)
        {
            throw new System.NotImplementedException();
        }

        public Task SaveEmailAsVerified(string email, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveEmailFromVerified(string email, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateEmailAsync(string email, string newEmail, string newPartnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<WalletDtoModel> CreateWalletAsync(string clientId, WalletType walletType, string name, string description)
        {
            throw new System.NotImplementedException();
        }

        public Task<WalletDtoModel> CreateWalletAsync(string clientId, WalletType walletType, OwnerType? owner, string name, string description)
        {
            throw new System.NotImplementedException();
        }

        public Task<WalletDtoModel> GetWalletAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetClientByWalletAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<WalletDtoModel> ModifyWalletAsync(string id, string name, string description)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteWalletAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<WalletDtoModel>> GetWalletsByClientIdAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<WalletDtoModel>> GetClientWalletsByTypeAsync(string clientId, WalletType walletType)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<WalletDtoModel>> GetClientWalletsFiltered(string clientId, WalletType? walletType = null, OwnerType? owner = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, IList<string>>> GetPartnerIdsAsync(string[] emails)
        {
            throw new System.NotImplementedException();
        }

        public Task<IDictionary<string, string>> GetClientsByPhonesAsync(string[] phoneNumbers)
        {
            throw new System.NotImplementedException();
        }

        public Task<ClientAccountInformationModel> RegisterAsync(string email, string phone, string password, string partnerId)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GenerateNotificationsId(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task BanClientAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task UnBanClientAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IList<BannedClient>> GetBannedClientsAsync(IList<string> clientIds = null)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsClientBannedAsync(string clientId)
        {
            throw new System.NotImplementedException();
        }

        public Task<int?> GetUsersCountByPartnerId(string partnerId)
        {
            throw new System.NotImplementedException();
        }
    }
}