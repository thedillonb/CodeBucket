using CodeBucket.Core.Data;
using System.Timers;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.Client;
using System.Threading.Tasks;

namespace CodeBucket.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly Timer _timer;

        public BitbucketClient Client { get; private set; }
		public BitbucketAccount Account { get; private set; }
        public IAccountsService Accounts { get; private set; }

        public ApplicationService(IAccountsService accounts)
        {
            Accounts = accounts;

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
                Accounts.Update(Account);

                Client = BitbucketClient.WithBearerAuthentication(Account.Token);
            }
            catch
            {
                // Do nothing....
            }
        }

		public void ActivateUser(BitbucketAccount account, BitbucketClient client)
        {
            Accounts.SetActiveAccount(account);
            Account = account;
            Client = client;
            _timer.Stop();
            _timer.Start();
        }
    }
}
