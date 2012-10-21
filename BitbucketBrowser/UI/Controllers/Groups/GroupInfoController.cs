using CodeFramework.UI.Controllers;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI.Controllers.Groups
{
    public class GroupInfoController : Controller<GroupModel>
    {
        public string User { get; private set; }
        
        public GroupInfoController(string user, GroupModel group)
            : base(true, true)
        {
            Style = UITableViewStyle.Plain;
            User = user;
            Model = group;
            Title = group.Name;
            EnableSearch = true;
            AutoHideSearch = true;
        }
        
        protected override void OnRefresh ()
        {
            var sec = new Section();
            Model.Members.OrderBy(x => x.Username).ToList().ForEach(s => {
                StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
                sec.Add(sse);
            });
            
            InvokeOnMainThread(delegate {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }
        
        protected override GroupModel OnUpdate(bool forced)
        {
            return Application.Client.Users[User].Groups[Model.Slug].GetInfo(forced);
        }
    }
}

