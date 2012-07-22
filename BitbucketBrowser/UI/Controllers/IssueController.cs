using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Drawing;
using MonoTouch.CoreGraphics;

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
        }

        protected override void OnRefresh ()
        {
            if (Model.Issues.Count == 0)
                return;

            var items = new List<Element>();
            Model.Issues.ForEach(x => {
                var el = new IssueElement(x);
                el.Tapped += () => {
                    //Make sure the first responder is gone.
                    View.EndEditing(true);
                    NavigationController.PushViewController(new IssueInfoController(User, Slug, x.LocalId), true);
                };
                items.Add(el);
            });

            InvokeOnMainThread(delegate {
                if (Root.Count == 0)
                {
                    var v = new RootElement(Title) { new Section() { Elements = items } };
                    v.UnevenRows = true;
                    Root = v;
                }
                else
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
        private readonly Section _comments, _details;
        private readonly MultilineElement _desc;
        private readonly SplitElement _split1, _split2;


        public IssueInfoController(string user, string slug, int id)
            : base(true, false)
        {
            User = user;
            Slug = slug;
            Id = id;

            Title = "Issue #" + id;

            Style = UITableViewStyle.Grouped;
            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            Root.Add(new Section(_header));

            _desc = new MultilineElement("") { PrimaryFont = UIFont.SystemFontOfSize(14f), BackgroundColor = UIColor.White };

            _split1 = new SplitElement(new SplitElement.Row() { Image1 = Images.Cog, Image2 = Images.Priority }) { BackgroundColor = UIColor.White };
            _split2 = new SplitElement(new SplitElement.Row() { Image1 = Images.Person, Image2 = Images.Flag }) { BackgroundColor = UIColor.White };

            _comments = new Section();
            _details = new Section() { _split1, _split2 };

            Root.Add(_details);
        }


        protected override void OnRefresh ()
        {

            _header.Title = Model.Issue.Title;
            _header.Subtitle = "Updated " + DateTime.Parse(Model.Issue.UtcLastUpdated).ToDaysAgo();
            var assigned = Model.Issue.Responsible != null ? Model.Issue.Responsible.Username : "unassigned";

            _split1.Value.Text1 = Model.Issue.Status;
            _split1.Value.Text2 = Model.Issue.Priority;
            _split2.Value.Text1 = assigned;
            _split2.Value.Text2 = Model.Issue.Metadata.Kind;

            _desc.Caption = Model.Issue.Content;

            var descValid = false;
            if (!string.IsNullOrEmpty(_desc.Caption))
            {
                _desc.Caption = _desc.Caption.Trim();
                _details.Insert(0, _desc);
                descValid = true;
            }

            var comments = new List<Element>(Model.Comments.Count);
            Model.Comments.ForEach(x => {
                if (!string.IsNullOrEmpty(x.Content))
                    comments.Add(new CommentElement() { 
                        Name = x.AuthorInfo.Username, 
                        Time = x.UtcCreatedOn, 
                        String = x.Content, 
                        Image = Images.Anonymous, 
                        ImageUri = new Uri(x.AuthorInfo.Avatar),
                        BackgroundColor = UIColor.White,
                    });
            });


            InvokeOnMainThread(delegate { 
                _header.SetNeedsDisplay(); 

                if (descValid)
                    Root.Reload(_desc, UITableViewRowAnimation.None);

                Root.Reload(_split1, UITableViewRowAnimation.None);
                Root.Reload(_split2, UITableViewRowAnimation.None);

                if (comments.Count == 0)
                {
                    Root.Remove(_comments);
                }
                else
                {
                    if (!Root.Contains(_comments))
                        Root.Add(_comments);
                    _comments.Clear();
                    _comments.Insert(0,  UITableViewRowAnimation.None, comments);
                }
            });
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

        public override UIView InputAccessoryView {
            get 
            {
                var u = new UIView(new RectangleF(0, 0, 320f, 27));
                u.BackgroundColor = UIColor.White;
                return u;
            }
        }

        private class CommentElement : NameTimeStringElement
        {
            protected override void OnCreateCell(UITableViewCell cell)
            {
                //Don't call the base since it will assign a background.
                return;
            }
        }
    }
}

