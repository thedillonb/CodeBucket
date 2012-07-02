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
            Root.Add(new Section());
        }

        protected override void OnRefresh ()
        {
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
        private readonly StyledStringElement _status, _type, _priority;


        public IssueInfoController(string user, string slug, int id)
            : base(true, false)
        {
            User = user;
            Slug = slug;
            Id = id;

            Style = UITableViewStyle.Grouped;
            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            Root.Add(new Section(_header));

            _desc = new MultilineElement("");

            _type = new StyledStringElement("Type", "", UITableViewCellStyle.Value1);
            _status = new StyledStringElement("Status", "", UITableViewCellStyle.Value1);
            _priority = new StyledStringElement("Priority", "", UITableViewCellStyle.Value1);

            _comments = new Section("Comments");
            _details = new Section() { _status, _type, _priority };

            Root.Add(_details);
        }


        protected override void OnRefresh ()
        {
            _header.Title = "#" + Model.Issue.LocalId + ": " + Model.Issue.Title;
            _header.Subtitle = "Updated " + DateTime.Parse(Model.Issue.UtcCreatedOn).ToDaysAgo();

            //Shall we show a picture?
            var type = Model.Issue.Metadata.Kind;
            if (type == "bug")
                _header.Image = IssueElement.BugImage;
            else if (type == "enhancement")
                _header.Image = IssueElement.EnhancementImage;
            else if (type == "proposal")
                _header.Image = IssueElement.ProposalImage;
            else if (type == "task")
                _header.Image = IssueElement.TaskImage;

            _type.Value = Model.Issue.Metadata.Kind;
            _status.Value = Model.Issue.Status;
            _priority.Value = Model.Issue.Priority;
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
                    comments.Add(new IssueComment(x));
            });


            InvokeOnMainThread(delegate { 
                _header.SetNeedsDisplay(); 

                if (descValid)
                    Root.Reload(_desc, UITableViewRowAnimation.None);

                Root.Reload(_type, UITableViewRowAnimation.None);
                Root.Reload(_status, UITableViewRowAnimation.None);
                Root.Reload(_priority, UITableViewRowAnimation.None);

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
    }

    public class IssueComment : CustomElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(13);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);


        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public IssueComment(CommentModel comment) : base(UITableViewCellStyle.Default, "changeelement")
        {
            Item = comment;
        }

        public CommentModel Item { get; set; }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            UIColor.Clear.SetFill();
            context.FillRect(bounds);

            var contentWidth = bounds.Width - LeftRightPadding * 2;

            var desc = Item.Content;

            string user;
            if (Item.AuthorInfo == null)
                user = "Unknown";
            else
                user = Item.AuthorInfo.Username;

            UIColor.FromRGB(0, 0x44, 0x66).SetColor();
            view.DrawString(user,
                new RectangleF(LeftRightPadding, TopBottomPadding, contentWidth, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
                );


            string daysAgo = DateTime.Parse(Item.UtcCreatedOn).ToDaysAgo();
            UIColor.FromRGB(0.6f, 0.6f, 0.6f).SetColor();
            float daysAgoTop = TopBottomPadding + UserFont.LineHeight;
            view.DrawString(
                daysAgo,
                new RectangleF(LeftRightPadding,  daysAgoTop, contentWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                var top = daysAgoTop + DateFont.LineHeight + 2f;
                view.DrawString(desc,
                    new RectangleF(LeftRightPadding, top, contentWidth, bounds.Height - TopBottomPadding - top), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            var contentWidth = bounds.Width - LeftRightPadding * 2 - 20f; //Account for the Accessory
            var descHeight = Item.Content.MonoStringHeight(DescFont, contentWidth);
            return TopBottomPadding*2 + UserFont.LineHeight + DateFont.LineHeight + 2f + descHeight;
        }
    }
}

