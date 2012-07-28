using System;
using System.Collections.Generic;
using System.Linq;

namespace BitbucketBrowser
{
    /// <summary>
    /// Application.
    /// </summary>
    public static class Application
    {
        public static BitbucketSharp.Client Client { get; private set; }
        public static Account Account { get; private set; }
        public static List<Account> Accounts { get; private set; }

        static Application()
        {
            Accounts = new List<Account>();
        }


        public static void SetUser(Account account)
        {
            Account = account;
            Client = new BitbucketSharp.Client(Account.Username, Account.Password);
        }

        public static void LoadSettings()
        {
            Accounts.Clear();

            //Linq query for accounts
            foreach (var a in Database.Main.Table<Account>().OrderBy(x => x.Id))
                Accounts.Add(a);
        }

        public static Account DefaultAccount()
        {
            return Accounts.Find(m => m.Id == Utils.Util.Defaults.IntForKey("DEFAULT_ACCOUNT"));
        }
    }
}

