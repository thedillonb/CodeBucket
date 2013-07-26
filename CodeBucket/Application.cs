using CodeBucket.Data;

namespace CodeBucket
{
    /// <summary>
    /// Application.
    /// </summary>
    public sealed class Application
    {
        public static BitbucketSharp.Client Client { get; private set; }

        public static CodeFramework.Data.Accounts<Account> Accounts { get; private set; }

        public static WebCacheProvider Cache { get; private set; }

        public static Account Account
        {
            get { return Accounts.ActiveAccount; }
            set { Accounts.ActiveAccount = value; }
        }

        static Application()
        {
            Accounts = new CodeFramework.Data.Accounts<Account>(Database.Main);
            Cache = new WebCacheProvider();
        }

        public static void SetUser(Account account)
        {
            if (account == null)
            {
                Account = null;
				Client = null;
                Accounts.SetDefault(null);
                return;
            }

            Account = account;
            Accounts.SetDefault(Account);
            
            //Release the cache
            Cache.DeleteAll();

			Client = new BitbucketSharp.Client(Account.Username, Account.Password) { 
				Timeout = 1000 * 30, //30 seconds
				CacheProvider = Cache,
			};
        }
    }
}

