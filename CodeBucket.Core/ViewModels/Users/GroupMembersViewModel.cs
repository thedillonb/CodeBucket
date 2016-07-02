using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Users
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

        protected override async Task Load()
        {
            var items = await this.GetApplication().Client.Groups.GetMembers(GroupName);
            Users.Items.Reset(items);
        }
    }
}

