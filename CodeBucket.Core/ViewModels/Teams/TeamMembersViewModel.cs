using CodeBucket.Core.ViewModels.User;
using System.Threading.Tasks;
using System.Linq;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseUserCollectionViewModel
    {
        public string Name { get; private set; }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        protected async override Task Load(bool forceCacheInvalidation)
        {
            var members = await Task.Run(() => this.GetApplication().Client.Teams[Name].GetMembers(forceCacheInvalidation));
            Users.Items.Reset(members.Values.Select(x => new UserModel { Avatar = x.Links.Avatar.Href, FirstName = x.DisplayName, IsTeam = false, Username = x.Username }));
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

