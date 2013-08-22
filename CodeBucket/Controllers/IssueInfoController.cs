using System;
using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using System.Collections.Generic;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;
using System.Linq;
using MonoTouch;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;
using CodeBucket.ViewControllers;

namespace CodeBucket.Controllers
{
    public class IssueInfoController : Controller<IssueInfoController.IssueInfoModel>
    {
        public int Id { get; private set; }
        public string User { get; private set; }
        public string Slug { get; private set; }
        public Action<IssueModel> ModelChanged;

        public IssueInfoController(IView<IssueInfoController.IssueInfoModel> view, string user, string slug, int id)
            : base(view)
        {
            User = user;
            Slug = slug;
            Id = id;
        }

        public override void Update(bool force)
        {
            var l = Application.Client.Users[User].Repositories[Slug].Issues[Id];
            Model = new IssueInfoController.IssueInfoModel {
                Comments = l.Comments.GetComments(force),
                Issue = l.GetIssue(force),
            };
        }

        public void EditComplete(IssueModel model)
        {
            Model.Issue = model;
            if (ModelChanged != null)
                ModelChanged(model);

            //If model == null then it has been deleted. No need to render.
            if (model != null)
                Render();
        }

        public void AddComment(string text)
        {
            var comment = Application.Client.Users[User].Repositories[Slug].Issues[Id].Comments.Create(new CommentModel { Content = text });
            Model.Comments.Add(comment);
            Render();
        }

        public class IssueInfoModel
        {
            public IssueModel Issue { get; set; }
            public List<CommentModel> Comments { get; set; }
        }
    }
}

