using System;
using SQLite;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using MonoTouch;
using BitbucketBrowser.Controllers.Issues;
using BitbucketBrowser.Controllers.Repositories;

namespace BitbucketBrowser.Data
{
    public class Account
    {
		/// <summary>
		/// The account type
		/// </summary>
		public enum Type : int
		{
			Bitbucket = 0,
			GitHub = 1
		}

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
		/// Gets or sets the type of the account.
		/// </summary>
		/// <value>The type of the account.</value>
		public Type AccountType { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BitbucketBrowser.Account"/> class.
		/// </summary>
		public Account()
		{
			//Set some default values
			DontRemember = false;
			AccountType = Type.Bitbucket;
		}
		
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
    }
}

