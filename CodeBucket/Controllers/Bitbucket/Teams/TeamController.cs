using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.UIKit;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;

namespace CodeBucket.Bitbucket.Controllers.Teams
{
    public class TeamController : ListController<TeamModel>
    {
        public TeamController(bool push = true) 
            : base(push)
        {
            Title = "Teams";
            SearchPlaceholder = "Search Teams";
        }

        protected override List<TeamModel> GetData(bool force, int currentPage, out int nextPage)
        {
            var items = Application.Client.Account.Teams.GetTeams();
            nextPage = -1;
            return items.OrderBy(a => a.DisplayName).ToList();
        }

        protected override Element CreateElement(TeamModel obj)
        {
            var element = new StyledElement(obj.DisplayName, () => NavigationController.PushViewController(new ProfileController(obj.Username), true));
            element.Image = Images.Anonymous;
            element.ImageUri = new System.Uri(obj.Avatar);
            return element;
        }
    }
}