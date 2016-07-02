using System.Threading.Tasks;
using CodeBucket.Client.Models;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Users
{
    public class UserFollowingsViewModel : BaseUserCollectionViewModel
    {
        public string Username { get; private set; }

        public void Init(NavObject navObject)
        {
            Username = navObject.Name;
        }

        protected override async Task Load()
        {
            var items = await this.GetApplication().Client.AllItems(x => x.Users.GetFollowing(Username));
            Users.Items.Reset(items.Select(x => new UserModel { Username = x.Username, Avatar = x.Links.Avatar.Href }).OrderBy(x => x.Username).ToList());
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}