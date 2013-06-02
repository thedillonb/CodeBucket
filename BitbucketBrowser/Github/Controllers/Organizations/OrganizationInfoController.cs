using BitbucketBrowser;
using CodeBucket.Controllers;
using GitHubSharp.Models;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

namespace CodeBucket.GitHub.Controllers.Organizations
{
    public class GroupInfoController : Controller<UserModel>
    {
        public string Org { get; private set; }
        
        public GroupInfoController(string org)
            : base(true, true)
        {
            Style = UITableViewStyle.Plain;
            Org = org;
            Title = org;
            EnableSearch = true;
            AutoHideSearch = true;
        }
        
        protected override void OnRefresh ()
        {
            var sec = new Section();

//            Model.OrderBy(x => x.Username).ToList().ForEach(s => {
//                StyledElement sse = new UserElement(s.Username, s.FirstName, s.LastName, s.Avatar);
//                sse.Tapped += () => NavigationController.PushViewController(new ProfileController(s.Username), true);
//                sec.Add(sse);
//            });
            
            InvokeOnMainThread(delegate {
                var root = new RootElement(Title) { sec };
                Root = root;
            });
        }

        protected override UserModel OnUpdate(bool forced)
        {
            return Application.GitHubClient.API.GetOrganization(Org).Data;
        }
    }
}
