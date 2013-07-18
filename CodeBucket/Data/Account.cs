using SQLite;
using CodeBucket.Bitbucket.Controllers.Issues;
using CodeBucket.Bitbucket.Controllers.Repositories;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace CodeBucket.Data
{
    public class Account
    {
		[PrimaryKey]
		[AutoIncrement]
		public int Id { get; set; }

		/// <summary>
		/// Gets or sets the username.
		/// </summary>
		/// <value>The username.</value>
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
		/// Gets or sets a value indicating whether this <see cref="Account"/> dont remember.
		/// THIS HAS TO BE A NEGATIVE STATEMENT SINCE IT DEFAULTS TO 'FALSE' WHEN RETRIEVING A NULL VIA SQLITE
		/// </summary>
		public bool DontRemember { get; set; }

        /// <summary>
        /// Gets or sets whether teams should be listed in the menu controller under 'events'
        /// </summary>
        public bool DontShowTeamEvents { get; set; }

        /// <summary>
        /// Gets or sets whether teams & groups should be expanded in the menu controller to their actual contents
        /// </summary>
        public bool DontExpandTeamsAndGroups { get; set; }

        /// <summary>
        /// A transient list of the teams this account is a part of
        /// </summary>
        [Ignore]
        public List<string> Teams { get; set; }

        /// <summary>
        /// A transient list of the groups this account is part of
        /// </summary>
        [Ignore]
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// A transient record of the user's name
        /// </summary>
        [Ignore]
        public string FullName { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Account"/> class.
		/// </summary>
		public Account()
		{
			//Set some default values
			DontRemember = false;
            DontShowTeamEvents = false;
            DontExpandTeamsAndGroups = false;

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
		/// Returns a <see cref="System.String"/> that represents the current <see cref="Account"/>.
		/// </summary>
		/// <returns>A <see cref="System.String"/> that represents the current <see cref="Account"/>.</returns>
		public override string ToString()
		{
			return Username;
		}

		/// <summary>
		/// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="Account"/>.
		/// </summary>
		/// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="Account"/>.</param>
		/// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
		/// <see cref="Account"/>; otherwise, <c>false</c>.</returns>
		public override bool Equals (object obj)
		{
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Account)) return false;
            var act = (Account)obj;
            return this.Id.Equals(act.Id);
		}

        /// <summary>
        /// Serves as a hash function for a <see cref="Gistacular.Data.Account"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode ()
        {
            return this.Id;
        }

        #region Filters

        private IssuesController.FilterModel _issueFilterModel;
        [Ignore]
        public IssuesController.FilterModel IssueFilterObject
        {
            get { return _issueFilterModel ?? (_issueFilterModel = MonoTouch.Configurations.Load<IssuesController.FilterModel>(Username, "IssueFilter")); }
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
            get { return _repoFilterModel ?? (_repoFilterModel = MonoTouch.Configurations.Load<RepositoryController.FilterModel>(Username, "RepoFilter")); }
            set
            {
                _repoFilterModel = value;
                MonoTouch.Configurations.Save(Username, "RepoFilter", _repoFilterModel);
            }
        }

        #endregion
    }
}

