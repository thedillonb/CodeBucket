using System;
using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch;
using MonoTouch.Foundation;
using CodeBucket.ViewControllers;

namespace CodeBucket.Controllers
{
    public class ChangesetInfoController : Controller<ChangesetInfoController.ChangesetInfoModel>
    {
        public string Node { get; private set; }

        public string User { get; private set; }

        public string Slug { get; private set; }

        public RepositoryDetailedModel Repo { get; set; }

        public ChangesetInfoController(IView<ChangesetInfoController.ChangesetInfoModel> view, string user, string slug, string node)
            : base(view)
        {
            Node = node;
            User = user;
            Slug = slug;
        }

        public override void Update(bool force)
        {
            var model = new ChangesetInfoController.ChangesetInfoModel();
            var x = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetInfo(force);
            x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            model.Changeset = x;

            //There is a bug that requires the 'rawNode'
            Node = x.RawNode;

            // Try to get these things
            try
            {
                model.Diffs = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetDiffs(force).ToDictionary(e => e.File);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get diffs", e);
            }

            try
            {
                model.Comments = Application.Client.Users[User].Repositories[Slug].Changesets[Node].Comments.GetComments(force);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get comments", e);
            }

            try
            {
                model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(force);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get likes", e);
            }

            Model = model;
        }

        public void Approve()
        {
            Application.Client.Users[User].Repositories[Slug].Changesets[Node].Approve();
            Model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
            Render();
        }

        public void Unapprove()
        {
            Application.Client.Users[User].Repositories[Slug].Changesets[Node].Unapprove();
            Model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
            Render();
        }

 
        public void AddComment(string text)
        {
            var comment = new CreateChangesetCommentModel { Content = text };
            var c = Application.Client.Users[User].Repositories[Slug].Changesets[Node].Comments.Create(comment);
            Model.Comments.Add(c);
            Render();
        }

        /// <summary>
        /// An inner class that combines two external models
        /// </summary>
        public class ChangesetInfoModel
        {
            public ChangesetModel Changeset { get; set; }
            public Dictionary<string, ChangesetDiffModel> Diffs { get; set; }
            public List<ChangesetCommentModel> Comments { get; set; }
            public List<ChangesetParticipantsModel> Likes { get; set; }

            public ChangesetInfoModel()
            {
                Diffs = new Dictionary<string, ChangesetDiffModel>();
                Comments = new List<ChangesetCommentModel>();
                Likes = new List<ChangesetParticipantsModel>();
            }
        }
    }
}

