using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.User;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Repositories
{
	public class WatchersViewModel : BaseUserCollectionViewModel
    {
        public string User
        {
            get;
            private set;
        }

        public string Repository
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            User = navObject.User;
            Repository = navObject.Repository;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			return Users.SimpleCollectionLoad(() => this.GetApplication().Client.Users[User].Repositories[Repository].GetFollowers(forceCacheInvalidation).Followers.Cast<UserModel>().OrderBy(x => x.Username).ToList());
        }

        public class NavObject
        {
            public string User { get; set; }
            public string Repository { get; set; }
        }
    }
}

