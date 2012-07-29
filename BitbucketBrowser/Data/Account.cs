using System;
using SQLite;
using System.Collections.Generic;
using System.Collections;

namespace BitbucketBrowser 
{
    public class Accounts : IEnumerable<Account>
    {

        public int Count 
        {
            get { return Database.Main.Table<Account>().Count(); }
        }

        public Account GetDefault()
        {
            var name = Utils.Util.Defaults.StringForKey("DEFAULT_ACCOUNT");
            if (name == null)
                return null;

            foreach (Account a in this)
                if (a.Username.ToLower().Equals(name.ToLower()))
                    return a;
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return GetEnumerator();
        }

        public IEnumerator<Account> GetEnumerator ()
        {
            return Database.Main.Table<Account>().GetEnumerator();
        }

        public void Insert(Account a)
        {
            Database.Main.Insert(a);
        }

        public void Remove(Account a)
        {
            Database.Main.Delete(a);
        }

        public void SetDefault(Account a)
        {
            Utils.Util.Defaults.SetString(a.Username, "DEFAULT_ACCOUNT");
        }

        public bool Exists(Account a)
        {
            foreach (var c in Database.Main.Table<Account>().Where(b => b.Username.ToLower().Equals(a.Username.ToLower())))
                return true;
            return false;
        }

    }


    public class Account
    {
        [PrimaryKey]
        public string Username { get; set; }

        public string Password { get; set; }

        public override string ToString()
        {
            return Username;
        }
    }
}

