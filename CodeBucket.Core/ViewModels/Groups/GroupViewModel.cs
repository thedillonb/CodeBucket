using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.User;
using System.Linq;

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
            return Users.SimpleCollectionLoad(() => 
                {
                    var groups = this.GetApplication().Client.Users[Username].Groups.GetGroups(forceCacheInvalidation);
                    var group = groups.FirstOrDefault(x => x.Name == GroupName);
                    if (group == null)
                        return new System.Collections.Generic.List<BitbucketSharp.Models.UserModel>();
                    return group.Members;
                });
        }

        public class NavObject
        {
			public string Username { get; set; }
			public string GroupName { get; set; }
        }
	}
}

