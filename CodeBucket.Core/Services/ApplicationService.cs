using CodeBucket.Core.Data;
using System.Linq;
using BitbucketSharp;
using System.Timers;
using CodeBucket.Core.ViewModels.Accounts;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly Timer _timer;

        public Client Client { get; private set; }
		public BitbucketAccount Account { get; private set; }
        public IAccountsService Accounts { get; private set; }

        public ApplicationService(IAccountsService accounts)
        {
            Accounts = accounts;

            _timer = new Timer(1000 * 60 * 45); // 45 minutes
            _timer.AutoReset = true;
            _timer.Elapsed += (sender, e) => {
                if (Account == null)
                    return;

                try
                {
                    var ret = Client.RefreshToken(LoginViewModel.ClientId, LoginViewModel.ClientSecret, Account.RefreshToken);
                    if (ret == null)
                        return;

                    Account.RefreshToken = ret.RefreshToken;
                    Account.Token = ret.AccessToken;
                    accounts.Update(Account);

                    UsersModel userInfo;
                    Client = Client.BearerLogin(Account.Token, out userInfo);
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
