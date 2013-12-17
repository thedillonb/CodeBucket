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

namespace CodeBucket.ViewControllers
{
    public class TeamViewController : BaseListControllerDrivenViewController, IListView<TeamController.TeamModel>
    {
        public TeamViewController() 
        {
            Title = "Teams".t();
            SearchPlaceholder = "Search Teams".t();
            NoItemsText = "No Teams".t();
            Controller = new TeamController(this);
        }

        public void Render(ListModel<TeamController.TeamModel> model)
        {
            RenderList(model, o => new StyledStringElement(o.Name, () => NavigationController.PushViewController(new ProfileViewController(o.Name), true)));
        }
    }
}