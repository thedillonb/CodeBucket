using CodeBucket.Data;

namespace CodeBucket
{
    /// <summary>
    /// Application.
    /// </summary>
    public static class Application
    {
        public static BitbucketSharp.Client Client { get; private set; }
		public static GitHubSharp.Client GitHubClient { get; private set; }

        public static Account Account { get; private set; }
        public static Accounts Accounts { get; private set; }
        public static WebCacheProvider Cache { get; private set; }

        static Application()
        {
            Accounts = new Accounts();
            Cache = new WebCacheProvider();
        }

        public static void SetUser(Account account)
        {
            if (account == null)
            {
                Account = null;
				Client = null;
				GitHubClient = null;
                Accounts.SetDefault(null);
                return;
            }

            Account = account;
            Accounts.SetDefault(Account);

			if (account.AccountType == Account.Type.Bitbucket)
			{
				GitHubClient = null;
				Client = new BitbucketSharp.Client(Account.Username, Account.Password) { 
					Timeout = 1000 * 30, //30 seconds
					CacheProvider = Cache,
				};
			}
			else if (account.AccountType == Account.Type.GitHub)
			{
				Client = null;
				GitHubClient = new GitHubSharp.Client(Account.Username, Account.Password) {
					Timeout = 1000 * 30, //30 seconds
					CacheProvider = Cache,
				};
			}
        }
    }
}

