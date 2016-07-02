using System.Threading.Tasks;
using System.Linq;
using CodeBucket.Client.Models;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Core.ViewModels.Teams
{
    public class TeamMembersViewModel : BaseUserCollectionViewModel
    {
        public string Name { get; private set; }

        public void Init(NavObject navObject)
        {
            Name = navObject.Name;
        }

        protected async override Task Load()
        {
            var members = await this.GetApplication().Client.Teams.GetMembers(Name);
            Users.Items.Reset(members.Values.Select(x => new UserModel { Avatar = x.Links.Avatar.Href, FirstName = x.DisplayName, IsTeam = false, Username = x.Username }));
        }

        public class NavObject
        {
            public string Name { get; set; }
        }
    }
}

