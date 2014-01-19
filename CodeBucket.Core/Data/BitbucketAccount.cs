using CodeFramework.Core.Data;
using SQLite;

namespace CodeBucket.Core.Data
{
    public class BitbucketAccount : Account
    {
		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		/// <value>The password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets whether teams should be listed in the menu controller under 'events'
        /// </summary>
		public bool DontShowTeamEvents { get; set; }

		[Ignore]
		public bool ShowTeamEvents
		{
			get { return !DontShowTeamEvents; }
			set { DontShowTeamEvents = !value; }
		}

        /// <summary>
        /// Gets or sets whether teams & groups should be expanded in the menu controller to their actual contents
        /// </summary>
		public bool DontExpandTeamsAndGroups { get; set; }

		[Ignore]
		public bool ExpandTeamsAndGroups
		{
			get { return !DontExpandTeamsAndGroups; }
			set { DontExpandTeamsAndGroups = !value; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CodeBucket.Data.Account"/> hides the repository
        /// description in list.
        /// </summary>
        /// <value><c>true</c> if hide repository description in list; otherwise, <c>false</c>.</value>
		public bool HideRepositoryDescriptionInList { get; set; }

		[Ignore]
		public bool RepositoryDescriptionInList
		{
			get { return !HideRepositoryDescriptionInList; }
			set { HideRepositoryDescriptionInList = !value; }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Account"/> class.
		/// </summary>
		public BitbucketAccount()
		{
			//Set some default values
			ShowTeamEvents = true;
			ExpandTeamsAndGroups = true;
			RepositoryDescriptionInList = true;
		}

    }
}