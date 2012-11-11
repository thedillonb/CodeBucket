using System;
using SQLite;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MonoTouch;

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
            var name = Utilities.Defaults.StringForKey("DEFAULT_ACCOUNT");
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

        public void Remove(string username)
        {
            var q = from f in Database.Main.Table<Account>()
                    where f.Username == username
                    select f;
            var account = q.FirstOrDefault();
            if (account != null)
                Remove(account);
        }

        public void SetDefault(Account a)
        {
            if (a == null)
                Utilities.Defaults.RemoveObject("DEFAULT_ACCOUNT");
            else
                Utilities.Defaults.SetString(a.Username, "DEFAULT_ACCOUNT");
            Utilities.Defaults.Synchronize();
        }

        public bool Exists(Account a)
        {
            var query = Database.Main.Query<Account>("select * from Account where LOWER(Username) = LOWER(?)", a.Username);
            return query.Count > 0;
        }

    }


    public class Account
    {
        [PrimaryKey]
        public string Username { get; set; }

        public string Password { get; set; }

        public string AvatarUrl { get; set; }

        public void Update()
        {
            Database.Main.Update(this);
        }

        public override string ToString()
        {
            return Username;
        }
    }
}

