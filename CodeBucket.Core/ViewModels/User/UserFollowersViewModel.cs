using System.Threading.Tasks;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.User
{
    public class UserFollowersViewModel : BaseUserCollectionViewModel
    {
        public string Name
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Username;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Users.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Name].GetFollowers(forceCacheInvalidation).Followers.Cast<UserModel>().OrderBy(x => x.Username).ToList());
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

