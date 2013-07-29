using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;

namespace CodeBucket.Bitbucket.Controllers.Teams
{
    public class TeamController : ModelDrivenController
    {
        public TeamController() 
            : base(typeof(List<string>))
        {
            Title = "Teams";
            SearchPlaceholder = "Search Teams";
            Style = UITableViewStyle.Plain;
        }

        protected override object OnUpdate(bool forced)
        {
            var model = Application.Client.Account.GetPrivileges(forced).Teams.Keys.OrderBy(a => a).ToList();
            model.Remove(Application.Account.Username); //Remove the current user from the 'teams'
            return model;
        }

        protected override void OnRefresh()
        {
            AddItems<string>(Model as List<string>, 
                             (o) => new StyledElement(o, () => NavigationController.PushViewController(new ProfileController(o), true)),
                             "No Teams");
        }
    }
}