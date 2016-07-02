using System.Threading.Tasks;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowersViewModel : BaseUserCollectionViewModel
    {
        public string Username { get; private set; }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        protected override async Task Load()
        {
            var items = await this.GetApplication().Client.Users.GetFollowers(Username);
            Users.Items.Reset(items.Followers.OrderBy(x => x.Username));
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}

