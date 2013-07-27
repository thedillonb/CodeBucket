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
    public class TeamController : BaseController
    {
        public List<string> Model { get; set; }

        public TeamController(bool push = true) 
            : base(push)
        {
            Title = "Teams";
            SearchPlaceholder = "Search Teams";
            Style = UITableViewStyle.Plain;
        }

        protected override async Task DoRefresh(bool force)
        {
            if (Model == null || force)
                await Task.Run(() => { 
                    Model = Application.Client.Account.GetPrivileges(force).Teams.Keys.OrderBy(a => a).ToList();
                    Model.Remove(Application.Account.Username); //Remove the current user from the 'teams'
                });
            AddItems<string>(Model, (o) => new StyledElement(o, () => NavigationController.PushViewController(new ProfileController(o), true)));
        }
    }
}