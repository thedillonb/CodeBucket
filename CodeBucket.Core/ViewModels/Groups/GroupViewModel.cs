using System.Threading.Tasks;
using System.Linq;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Core.ViewModels.Groups
{
	public class GroupViewModel : BaseUserCollectionViewModel
	{
		public string Username { get; private set; }

		public string GroupName { get; private set; }

		public void Init(NavObject navObject) 
		{
			Username = navObject.Username;
			GroupName = navObject.GroupName;
		}

        protected override async Task Load()
        {
            var groups = await this.GetApplication().Client.Groups.GetGroups(Username);
            var group = groups.FirstOrDefault(x => x.Name == GroupName);

            if (group == null)
                Users.Items.Clear();
            else
                Users.Items.Reset(group.Members);
        }

        public class NavObject
        {
			public string Username { get; set; }
			public string GroupName { get; set; }
        }
	}
}

