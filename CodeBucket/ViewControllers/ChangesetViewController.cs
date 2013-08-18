using CodeFramework.Controllers;
using CodeBucket.Controllers;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using System;

namespace CodeBucket.ViewControllers
{
    public class ChangesetViewController : BaseListControllerDrivenViewController, IListView<ChangesetModel>
    {
        private readonly string _user;
        private readonly string _slug;

        public ChangesetViewController(string user, string slug)
        {
            _user = user;
            _slug = slug;
            Title = "Changes".t();
            Root.UnevenRows = true;
            Controller = new ChangesetController(this, user, slug);
        }

        public void Render(ListModel<ChangesetModel> model)
        {
            RenderList(model, x => {
                var desc = (x.Message ?? "").Replace("\n", " ").Trim();
                var el = new NameTimeStringElement { Name = x.Author, Time = (x.Utctimestamp.ToDaysAgo()), String = desc, Lines = 4 };
                el.Tapped += () => NavigationController.PushViewController(new ChangesetInfoViewController(_user, _slug, x.Node), true);
                return el;
            });
        }
    }
}

