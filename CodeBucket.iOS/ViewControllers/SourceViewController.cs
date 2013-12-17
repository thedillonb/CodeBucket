using CodeBucket.Bitbucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Filters.Models;
using CodeBucket.Controlleres;

namespace CodeBucket.ViewControllers
{
    public class SourceViewController : BaseListControllerDrivenViewController, IListView<SourceController.SourceModel>
    {
        private readonly string _username;
        private readonly string _slug;
        private readonly string _branch;
        private readonly string _path;

        public new SourceController Controller
        {
            get { return (SourceController)base.Controller; }
            protected set { base.Controller = value; }
        }

        public SourceViewController(string username, string slug, string branch = "master", string path = "")
        {
            _username = username;
            _slug = slug;
            _branch = branch;
            _path = path;
            EnableSearch = true;
            EnableFilter = true;
            SearchPlaceholder = "Search Files & Folders".t();
            Title = string.IsNullOrEmpty(path) ? "Source".t() : path.Substring(path.LastIndexOf('/') + 1);
            Controller = new SourceController(this, username, slug, branch, path);
        }

        public void Render(ListModel<SourceController.SourceModel> model)
        {
            RenderList(model, x => {
                if (x.IsFile)
                {
                    var i = x.Name.LastIndexOf('/') + 1;
                    var p = x.Name.Substring(i);
                    return new StyledStringElement(p, () => NavigationController.PushViewController(new SourceInfoViewController(_username, _slug, _branch, x.Name) { Title = p }, true), Images.File);
                }
                else
                    return new StyledStringElement(x.Name, () => NavigationController.PushViewController(new SourceViewController(_username, _slug, _branch, _path + "/" + x.Name), true), Images.Folder);
            });
        }

        protected override CodeFramework.Filters.Controllers.FilterViewController CreateFilterController()
        {
            return new CodeBucket.Filters.ViewControllers.SourceFilterViewController(Controller);
        }
    }
}

