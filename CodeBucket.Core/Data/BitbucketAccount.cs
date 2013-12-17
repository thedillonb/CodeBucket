using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitbucketSharp.Models;
using CodeFramework.Core.Data;
using SQLite;

namespace CodeBucket.Core.iOS.Data
{
    public class BitbucketAccount : Account
    {
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
        public string Password { get; set; }

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
        /// Gets or sets a value indicating whether this <see cref="CodeBucket.Data.Account"/> hides the repository
        /// description in list.
        /// </summary>
        /// <value><c>true</c> if hide repository description in list; otherwise, <c>false</c>.</value>
        public bool HideRepositoryDescriptionInList { get; set; }

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
		public BitbucketAccount()
		{
			//Set some default values
			DontRemember = false;
            DontShowTeamEvents = false;
            DontExpandTeamsAndGroups = false;
		}

    }
}