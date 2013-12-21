using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.User
{
    public class GroupMembersViewModel : BaseUserCollectionViewModel
    {
        public string GroupName 
        { 
            get; 
            private set; 
        }

		public string Username { get; set; }

        public void Init(NavObject navObject)
        {
			Username = navObject.Username;
            GroupName = navObject.GroupName;
        }

        public class NavObject
        {
			public string Username { get; set; }
            public string GroupName { get; set; }
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Users.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Groups[GroupName].GetInfo(forceCacheInvalidation).Members);
        }
    }
}

