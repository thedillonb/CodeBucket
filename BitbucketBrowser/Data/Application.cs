using MonoTouch;

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

        public static bool AutoSignin
        {
            get
            {
                //If it's never been set.
                if (Utilities.Defaults.ValueForKey(new MonoTouch.Foundation.NSString("AUTO_SIGN_IN")) == null)
                {
                    AutoSignin = true;
                    return true;
                }

                var b = Utilities.Defaults.BoolForKey("AUTO_SIGN_IN");
                return b;
            }
            set
            {
                Utilities.Defaults.SetBool(value, "AUTO_SIGN_IN");
                Utilities.Defaults.Synchronize();
            }
        }
    }
}

