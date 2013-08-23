using CodeBucket.Data;

namespace CodeBucket
{
    /// <summary>
    /// Application.
    /// </summary>
    public sealed class Application
    {
        public static BitbucketSharp.Client Client { get; private set; }

        public static Accounts Accounts { get; private set; }

        public static Account Account
        {
            get { return Accounts.ActiveAccount; }
            set { Accounts.ActiveAccount = value; }
        }

        static Application()
        {
            Accounts = new Accounts(Database.Main);
        }

        public static void UnsetUser()
        {
            Account = null;
            Client = null;
            Accounts.SetDefault(null);
        }

        public static void SetUser(Account account, BitbucketSharp.Client client)
        {
            Account = account;
            Accounts.SetDefault(Account);

            //Assign the client
            Client = client;
            Client.Timeout = 1000 * 30;
            Client.CacheProvider = new AppCache();
        }

        /// <summary>
        /// A cache provider for BitBucketSharp.
        /// Since the CodeFramework.Data.WebCacheProvider was modeled directly after the interface
        /// it can just inherit both and be alright. Otherwise, i'd have to do a little bit of work to make
        /// the proxy class.
        /// </summary>
        private class AppCache : CodeFramework.Data.WebCacheProvider, BitbucketSharp.ICacheProvider
        {
        }
    }
}

