using System.Threading.Tasks;
using BitbucketSharp.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.User
{
    public class UserFollowingsViewModel : BaseUserCollectionViewModel
    {
        public string Name
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Users.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Name].GetFollowing(forceCacheInvalidation).Values.Select(x => new UserModel { Username = x.Username, Avatar = x.Links.Avatar.Href }).OrderBy(x => x.Username).ToList());
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}