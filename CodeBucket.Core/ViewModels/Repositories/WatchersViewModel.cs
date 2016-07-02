using System.Threading.Tasks;
using System.Linq;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Core.ViewModels.Repositories
{
	public class WatchersViewModel : BaseUserCollectionViewModel
    {
        public string User { get; private set; }

        public string Repository { get; private set; }

        public void Init(NavObject navObject)
        {
            User = navObject.User;
            Repository = navObject.Repository;
        }

        protected override async Task Load()
        {
            var items = await this.GetApplication().Client.Repositories.GetFollowers(User, Repository);
            Users.Items.Reset(items.Followers.OrderBy(x => x.Username));
        }

        public class NavObject
        {
            public string User { get; set; }
            public string Repository { get; set; }
        }
    }
}

