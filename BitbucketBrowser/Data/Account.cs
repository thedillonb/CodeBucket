using System;
using SQLite;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MonoTouch;
using BitbucketBrowser.UI.Controllers.Issues;
using BitbucketBrowser.UI.Controllers.Repositories;

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
            a.Delete();
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

		public Account Find(string username)
		{
			var query = Database.Main.Query<Account>("select * from Account where LOWER(Username) = LOWER(?)", username);
			if (query.Count > 0)
				return query[0];
			return null;
		}
    }


    public class Account
    {
		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
        [PrimaryKey]
        public string Username { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
        public string Password { get; set; }

		/// <summary>
		/// Gets or sets the avatar URL.
		/// </summary>
		/// <value>The avatar URL.</value>
        public string AvatarUrl { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="BitbucketBrowser.Account"/> dont remember.
		/// THIS HAS TO BE A NEGATIVE STATEMENT SINCE IT DEFAULTS TO 'FALSE' WHEN RETRIEVING A NULL VIA SQLITE
		/// </summary>
		public bool DontRemember { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BitbucketBrowser.Account"/> class.
		/// </summary>
		public Account()
		{
			DontRemember = true;
		}


        #region Filters

        private IssuesController.FilterModel _issueFilterModel;
        [Ignore]
        public IssuesController.FilterModel IssueFilterObject
        {
            get
            {
                if (_issueFilterModel == null)
                    _issueFilterModel = MonoTouch.Configurations.Load<IssuesController.FilterModel>(Username, "IssueFilter");
                return _issueFilterModel;
            }
            set
            {
                _issueFilterModel = value;
                MonoTouch.Configurations.Save(Username, "IssueFilter", _issueFilterModel);
            }
        }

        private RepositoryController.FilterModel _repoFilterModel;
        [Ignore]
        public RepositoryController.FilterModel RepoFilterObject
        {
            get
            {
                if (_repoFilterModel == null)
                    _repoFilterModel = MonoTouch.Configurations.Load<RepositoryController.FilterModel>(Username, "RepoFilter");
                return _repoFilterModel;
            }
            set
            {
                _repoFilterModel = value;
                MonoTouch.Configurations.Save(Username, "RepoFilter", _repoFilterModel);
            }
        }

        #endregion

		/// <summary>
		/// Delete this instance in the database
		/// </summary>
        public void Delete()
        {
            //Delete configurations
            MonoTouch.Configurations.Delete(Username);

            Database.Main.Delete(this);
        }

		/// <summary>
		/// Update this instance in the database
		/// </summary>
        public void Update()
        {
            Database.Main.Update(this);
        }

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents the current <see cref="BitbucketBrowser.Account"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="BitbucketBrowser.Account"/>.</returns>
        public override string ToString()
        {
            return Username;
        }
    }
}

