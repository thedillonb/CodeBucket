using MonoTouch;
using BitbucketBrowser.Data;

namespace BitbucketBrowser
{
    /// <summary>
    /// Application.
    /// </summary>
    public static class Application
    {
        public static BitbucketSharp.Client Client { get; private set; }
        public static Account Account { get; private set; }
        public static Accounts Accounts { get; private set; }
        public static Data.WebCacheProvider Cache { get; private set; }

        static Application()
        {
            Accounts = new Accounts();
            Cache = new Data.WebCacheProvider();
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
            Client = new BitbucketSharp.Client(Account.Username, Account.Password) { 
                Timeout = 1000 * 30,
                CacheProvider = Cache,
            };
        }
    }
}

