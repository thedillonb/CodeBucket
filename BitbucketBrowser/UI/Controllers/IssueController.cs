using System;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BitbucketBrowser.Utils;
using System.Drawing;
using MonoTouch.CoreGraphics;
using RedPlum;
using System.Threading;
using MonoTouch.Foundation;

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

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => {
                var b = new IssueEditController();
                NavigationController.PushViewController(b, true);
            });
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

        private bool _scrollToLastComment = false;

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

            var addComment = new StyledElement("Add Comment", Images.Pencil);
            addComment.Tapped += AddCommentTapped;


            _comments = new Section();
            _details = new Section() { _split1, _split2 };

            Root.Add(_details);
            Root.Add(_comments);
            Root.Add(new Section() { addComment });
        }

        void AddCommentTapped ()
        {
            var composer = new Composer();
            composer.NewComment(this, () => {

                MBProgressHUD hud = null;
                hud = new MBProgressHUD(this.View.Superview); 
                hud.Mode = MBProgressHUDMode.Indeterminate;
                hud.TitleText = "Posting...";
                this.View.Superview.AddSubview(hud);
                hud.Show(true);

                var text = composer.Text;

                ThreadPool.QueueUserWorkItem(delegate {

                    try
                    {
                        Application.Client.Users[User].Repositories[Slug].Issues[Id].Comments.Create(new CommentModel() {
                            Content = text
                        });
                    }
                    catch (Exception)
                    {
                    }

                    InvokeOnMainThread(delegate {

                        hud.Hide(true);
                        hud.RemoveFromSuperview();

                        composer.CloseComposer();

                        _scrollToLastComment = true;

                        //Clear the model and don't force so the spiny wheel will pop up!
                        Model = null;
                        Refresh();
                    });
                });
            });

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
                if (_desc.Parent == null)
                    _details.Insert(0, _desc);
                descValid = true;
            }

            var comments = new List<Element>(Model.Comments.Count);
            Model.Comments.OrderBy(x => DateTime.Parse(x.UtcCreatedOn)).ToList().ForEach(x => {
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

                _comments.Clear();
                _comments.Insert(0,  UITableViewRowAnimation.None, comments);

                if (_scrollToLastComment && _comments.Elements.Count > 0)
                {
                    TableView.ScrollToRow(NSIndexPath.FromRowSection(_comments.Elements.Count - 1, 2), UITableViewScrollPosition.Top, true);
                    _scrollToLastComment = false;
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

    public class IssueEditController : Controller<object>
    {
        public IssueEditController()
            : base(true, false)
        {
            Model = this;

            var title = new EntryElement("Title", "Issue Title", string.Empty);
            var content = new MultiLineEntryElement("Content", string.Empty) { Rows = 4 };

            var root = new RootElement("New Issue");
            root.Add(new Section() { title, content });

            Root = root;
        }



        protected override void OnRefresh ()
        {
        }

        protected override object OnUpdate ()
        {
            return this;
        }
    }
}

