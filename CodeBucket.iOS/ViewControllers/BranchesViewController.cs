using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using BitbucketSharp.Models;
using MonoTouch.Dialog;

namespace CodeBucket.ViewControllers
{
    public class BranchesViewController : BaseListControllerDrivenViewController, IListView<BranchModel>
    {
        private readonly string _username;
        private readonly string _slug;

        public BranchesViewController(string username, string slug)
        {
            _username = username;
            _slug = slug;
            Title = "Branches".t();
            SearchPlaceholder = "Search Branches".t();
            NoItemsText = "No Branches".t();
            Controller = new BranchesController(this, username, slug);
        }

        public void Render(ListModel<BranchModel> model)
        {
            RenderList(model, x => {
                return new StyledStringElement(x.Branch, () => NavigationController.PushViewController(new SourceViewController(_username, _slug, x.Branch), true));
            });
        }
    }
}
