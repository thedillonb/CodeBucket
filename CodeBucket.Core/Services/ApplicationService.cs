using CodeBucket.Core.Data;
using System.Timers;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Client;
using System.Threading.Tasks;

namespace CodeBucket.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IDefaultValueService _defaultValueService;
        private readonly IAccountsService _accountsService;
        private readonly Timer _timer;

        public BitbucketClient Client { get; private set; }
		public Account Account { get; private set; }

        public ApplicationService(IAccountsService accounts, IDefaultValueService defaultValueService)
        {
            _accountsService = accounts;
            _defaultValueService = defaultValueService;

            _timer = new Timer(1000 * 60 * 45); // 45 minutes
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, e) => RefreshToken().RunSynchronously();
        }

        public async Task RefreshToken()
        {
            try
            {
                if (Account == null)
                    return;
                
                var ret = await BitbucketClient.GetRefreshToken(LoginViewModel.ClientId, LoginViewModel.ClientSecret, Account.RefreshToken);
                if (ret == null)
                    return;

                Account.RefreshToken = ret.RefreshToken;
                Account.Token = ret.AccessToken;
                _accountsService.Save(Account).ToBackground();

                Client = BitbucketClient.WithBearerAuthentication(Account.Token);
            }
            catch
            {
                // Do nothing....
            }
        }

        public async Task SaveAccount()
        {
            if (Account == null)
                return;

            await _accountsService.Save(Account);
        }

        public void SetDefaultAccount(Account account)
        {
            if (account == null)
                _defaultValueService.Clear("DEFAULT_ACCOUNT");
            else
                _defaultValueService.Set("DEFAULT_ACCOUNT", account.Key);
        }

        public async Task<Account> GetDefaultAccount()
        {
            string id;
            if (!_defaultValueService.TryGet("DEFAULT_ACCOUNT", out id))
                return null;
            return await _accountsService.Get(id);
        }

        public void ActivateUser(Account account, BitbucketClient client)
        {
            SetDefaultAccount(account);
            Account = account;
            Client = client;
            _timer.Stop();
            _timer.Start();
        }
    }
}
