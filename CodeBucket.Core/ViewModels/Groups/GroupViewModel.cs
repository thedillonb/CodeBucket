using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.User;

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

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Users.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Groups[GroupName].GetInfo(forceCacheInvalidation).Members);
        }

        public class NavObject
        {
			public string Username { get; set; }
			public string GroupName { get; set; }
        }
	}
}

