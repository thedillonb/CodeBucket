using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Groups
{
    public class GroupInfoController : BaseController
    {
        public GroupModel Model { get; set; }
        public string User { get; private set; }
        public string GroupName { get; private set; }
        
        public GroupInfoController(string user, string groupName)
            : base(true, true)
        {
            Style = UITableViewStyle.Plain;
            User = user;
            EnableSearch = true;
            AutoHideSearch = true;
            SearchPlaceholder = "Search Memebers";
            Title = groupName;
            GroupName = groupName;
        }

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { Model = Application.Client.Users[User].Groups[GroupName].GetInfo(force); });

            AddItems<UserModel>(Model.Members.OrderBy(x => x.Username).ToList(), (s) => {
                StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                return sse;
            });
        }
    }
}

