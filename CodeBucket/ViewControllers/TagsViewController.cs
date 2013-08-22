using System;
using CodeFramework.Controllers;
using CodeBucket.Controllers;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;

namespace CodeBucket.ViewControllers
{
    public class TagsViewController : BaseListControllerDrivenViewController, IListView<TagsController.TagModel>
    {
        private readonly string _username;
        private readonly string _slug;

        public TagsViewController(string username, string slug)
        {
            _username = username;
            _slug = slug;
            Title = "Tags".t();
            SearchPlaceholder = "Search Tags".t();
            NoItemsText = "No Tags".t();
            EnableSearch = true;
            Controller = new TagsController(this, username, slug);
        }

        public void Render(ListModel<TagsController.TagModel> model)
        {
            RenderList(model, x => new StyledStringElement(x.Name, () => NavigationController.PushViewController(new SourceViewController(_username, _slug, x.Node), true)));
        }
    }
}

