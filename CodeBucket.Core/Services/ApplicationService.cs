using CodeBucket.Core.Data;
using BitbucketSharp;
using System.Timers;
using CodeBucket.Core.ViewModels.Accounts;
using System.Net.Http;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly Timer _timer;

        public Client Client { get; private set; }
		public BitbucketAccount Account { get; private set; }
        public IAccountsService Accounts { get; private set; }

        public ApplicationService(IAccountsService accounts, ILoadingIndicatorService loadingService)
        {
            Accounts = accounts;

            Client.Factory = () => new HttpClient(new LoadingMessageHandler(loadingService));

            _timer = new Timer(1000 * 60 * 30); // 30 minutes
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, e) => {
                if (Account == null)
                    return;

                try
                {
                    var ret = Client.GetRefreshToken(LoginViewModel.ClientId, LoginViewModel.ClientSecret, Account.RefreshToken).Result;
                    if (ret == null)
                        return;

                    Account.RefreshToken = ret.RefreshToken;
                    Account.Token = ret.AccessToken;
                    accounts.Update(Account);

                    Client = Client.WithBearerAuthentication(Account.Token);
                }
                catch
                {
                    // Do nothing....
                }
            };
        }

		public void ActivateUser(BitbucketAccount account, Client client)
        {
            Accounts.SetActiveAccount(account);
            Account = account;
            Client = client;
            _timer.Stop();
            _timer.Start();
        }
    }
}
