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
			CheckCacheSize(account.Cache);

            _timer.Stop();
            _timer.Start();
        }

		private static void CheckCacheSize(CodeFramework.Core.Data.AccountCache cache)
		{
			var totalCacheSize = cache.Sum(x => System.IO.File.Exists(x.Path) ? new System.IO.FileInfo(x.Path).Length : 0);
			var totalCacheSizeMB = ((float)totalCacheSize / 1024f / 1024f);

			if (totalCacheSizeMB > 64)
			{
				System.Console.WriteLine("Flushing cache due to size...");
				cache.DeleteAll();
			}
		}

//		private class GitHubCache : ICache
//		{
//			private readonly CodeFramework.Core.Data.AccountCache _account;
//			public GitHubCache(GitHubAccount account)
//			{
//				_account = account.Cache;
//			}
//
//			public string GetETag(string url)
//			{
//				var data = _account.GetEntry(url);
//				if (data == null)
//					return null;
//				return data.CacheTag;
//			}
//
//			public T Get<T>(string url) where T : new()
//			{
//				var data = _account.Get<T>(url);
//				if (data == null)
//					return default(T);
//
//				System.Console.WriteLine("[GET] cache: {0}", url);
//				return data;
//			}
//
//			public void Set(string url, object data, string etag)
//			{
//				System.Console.WriteLine("[SET] cache: {0}", url);
//				_account.Set(url, data, etag);
//			}
//
//			public bool Exists(string url)
//			{
//				return _account.GetEntry(url) != null;
//			}
//		}
    }
}
