using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace BitbucketBrowser.UI
{
    public class IssuesController : Controller<IssuesModel>
    {
        public string User { get; private set; }

        public string Slug { get; private set; }

        private DateTime _lastUpdate = DateTime.MinValue;

        public IssuesController(string user, string slug)
            : base(true, true)
        {
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Plain;
            Title = "Issues";
            EnableSearch = true;
            AutoHideSearch = true;

            Root.UnevenRows = true;
            Root.Add(new Section());
        }

        protected override void OnRefresh ()
        {
            var items = new List<Element>();
            Model.Issues.ForEach(x => {
                var el = new IssueElement(x);
                el.Tapped += () => NavigationController.PushViewController(new IssueInfoController(User, Slug, x.LocalId), true);
                items.Add(el);
            });

            InvokeOnMainThread(delegate {
                Root[0].Insert(0, UITableViewRowAnimation.Top, items);
            });
        }

        protected override IssuesModel OnUpdate ()
        {
            var issues = Application.Client.Users[User].Repositories[Slug].Issues.GetIssues();

            var newChanges =
                         (from s in issues.Issues
                          where DateTime.Parse(s.UtcCreatedOn) > _lastUpdate
                          orderby DateTime.Parse(s.UtcCreatedOn) descending
                          select s).ToList();
            if (newChanges.Count > 0)
                 _lastUpdate = (from r in newChanges select DateTime.Parse(r.UtcCreatedOn)).Max();

            issues.Issues = newChanges;
            return issues;
        }
    }

    public class IssueInfoController : Controller<IssueInfoController.InternalModel>
    {
        public int Id { get; private set; }
        public string User { get; private set; }
        public string Slug { get; private set; }

        public class InternalModel
        {
            public IssueModel Issue { get; set; }
            public List<CommentModel> Comments { get; set; } 
        };

        private readonly HeaderView _header;

        public IssueInfoController(string user, string slug, int id)
            : base(true, false)
        {
            User = user;
            Slug = slug;
            Id = id;

            Style = UITableViewStyle.Grouped;
            _header = new HeaderView(View.Bounds.Width) { Title = "#" + id };
            Root.Add(new Section(_header));
        }


        protected override void OnRefresh ()
        {
            _header.Subtitle = Model.Issue.Title;

            InvokeOnMainThread(delegate { _header.SetNeedsDisplay(); });
        }

        protected override InternalModel OnUpdate ()
        {
            var l = Application.Client.Users[User].Repositories[Slug].Issues[Id];
            var m = new InternalModel() {
                Comments = l.Comments.GetComments(),
                Issue = l.GetIssue()
            };
            return m;
        }
    }
}

