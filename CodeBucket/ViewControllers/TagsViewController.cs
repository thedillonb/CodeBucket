using System;
using CodeFramework.Controllers;
using CodeBucket.Controllers;
using System.Collections.Generic;
using MonoTouch.Dialog;
using System.Linq;
using CodeBucket.Bitbucket.Controllers.Source;

namespace CodeBucket.ViewControllers
{
    public class TagsViewController : ListView, IView<List<TagsController.TagModel>>
    {
        private readonly string _username;
        private readonly string _slug;

        public new TagsController Controller
        {
            get { return (TagsController)base.Controller; }
            protected set { base.Controller = value; }
        }

        public TagsViewController(string username, string slug)
        {
            Title = username;
            _username = username;
            _slug = slug;
            Controller = new TagsController(this, username, slug);
        }

        void IView<List<TagsController.TagModel>>.Render(List<TagsController.TagModel> model)
        {
            RenderList(model, x => new StyledStringElement(x.Name, () => NavigationController.PushViewController(new SourceController(_username, _slug, x.Node), true)));
        }
    }
}

