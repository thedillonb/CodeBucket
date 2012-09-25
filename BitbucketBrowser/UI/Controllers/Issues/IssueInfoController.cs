using System;
using System.Threading;
using CodeFramework.UI.Controllers;
using BitbucketSharp.Models;
using System.Collections.Generic;
using CodeFramework.UI.Views;
using MonoTouch.Dialog;
using CodeFramework.UI.Elements;
using MonoTouch.UIKit;
using RedPlum;
using System.Drawing;
using MonoTouch.Foundation;
using System.Linq;
using MonoTouch;

namespace BitbucketBrowser.UI.Controllers.Issues
{
    public class InternalIssueInfoModel
    {
        public IssueModel Issue { get; set; }
        public List<CommentModel> Comments { get; set; } 
    }

    public class IssueInfoController : Controller<InternalIssueInfoModel>
    {
        public int Id { get; private set; }
        public string User { get; private set; }
        public string Slug { get; private set; }
        public Action<IssueModel> ModelChanged;

        private readonly HeaderView _header;
        private readonly Section _comments, _details;
        private readonly CodeFramework.UI.Elements.MultilinedElement _desc;
        private readonly SplitElement _split1, _split2, _split3;
        private readonly StyledElement _responsible;
        
        private bool _scrollToLastComment = false;
        private bool _issueRemoved = false;
        
        public IssueInfoController(string user, string slug, int id)
            : base(true, false)
        {
            User = user;
            Slug = slug;
            Id = id;
            Title = "Issue #" + id;

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (s, e) => {
                var editController = new IssueEditController() {
                    ExistingIssue = Model.Issue,
                    Username = User,
                    RepoSlug = Slug,
                    Title = "Edit Issue",
                };
                editController.Success = EditingComplete;
                NavigationController.PushViewController(editController, true);
            });
            NavigationItem.RightBarButtonItem.Enabled = false;
            
            Style = UITableViewStyle.Grouped;
            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            Root.Add(new Section(_header));
            
            _desc = new CodeFramework.UI.Elements.MultilinedElement("") { BackgroundColor = UIColor.White };
            _desc.CaptionFont = _desc.ValueFont;
            _desc.CaptionColor = _desc.ValueColor;
            
            _split1 = new SplitElement(new SplitElement.Row() { Image1 = Images.Cog, Image2 = Images.Priority }) { BackgroundColor = UIColor.White };
            _split2 = new SplitElement(new SplitElement.Row() { Image1 = Images.Flag, Image2 = Images.ServerComponents }) { BackgroundColor = UIColor.White };
            _split3 = new SplitElement(new SplitElement.Row() { Image1 = Images.SitemapColor, Image2 = Images.Milestone }) { BackgroundColor = UIColor.White };

            _responsible = new StyledElement("Unassigned", Images.Person) {
                Font = StyledElement.DefaultDetailFont,
                TextColor = StyledElement.DefaultDetailColor,
            };
            _responsible.Tapped += () => {
                if (Model != null && Model.Issue.Responsible != null)
                    NavigationController.PushViewController(new ProfileController(Model.Issue.Responsible.Username, true), true);
            };

            var addComment = new StyledElement("Add Comment", Images.Pencil);
            addComment.Tapped += AddCommentTapped;
            
            _comments = new Section();
            _details = new Section() { _split1, _split2, _split3, _responsible };
            
            Root.Add(_details);
            Root.Add(_comments);
            Root.Add(new Section() { addComment });
        }

        void EditingComplete(IssueModel model)
        {
            if (ModelChanged != null)
                ModelChanged(model);

            //If it's null then we've deleted it!
            if (model == null)
            {
                _issueRemoved = true;
            }
            //Otherwise let's just reassign the model and call the OnRefresh to update the screen!
            else
            {
                Model.Issue = model;
                OnRefresh();
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_issueRemoved)
            {
                NavigationController.PopViewControllerAnimated(true);
            }
        }
        
        void AddCommentTapped ()
        {
            var composer = new Composer();
            composer.NewComment(this, () => {
                var comment = new CommentModel() { Content = composer.Text };

                composer.DoWork(() => {
                    Application.Client.Users[User].Repositories[Slug].Issues[Id].Comments.Create(comment);

                    InvokeOnMainThread(() => {
                        composer.CloseComposer();
                        _scrollToLastComment = true;
                        Model = null;
                        Refresh();
                    });
                }, (ex) => {
                    Utilities.ShowAlert("Unable to post comment!", ex.Message);
                    composer.EnableSendButton = true;
                });
            });
        }
        
        protected override void OnRefresh()
        {
            BeginInvokeOnMainThread(() => { NavigationItem.RightBarButtonItem.Enabled = true; });

            _header.Title = Model.Issue.Title;
            _header.Subtitle = "Updated " + DateTime.Parse(Model.Issue.UtcLastUpdated).ToDaysAgo();
            _split1.Value.Text1 = Model.Issue.Status;
            _split1.Value.Text2 = Model.Issue.Priority;
            _split2.Value.Text1 = Model.Issue.Metadata.Kind;
            _split2.Value.Text2 = Model.Issue.Metadata.Component ?? "No Component";
            _split3.Value.Text1 = Model.Issue.Metadata.Version ?? "No Version";
            _split3.Value.Text2 = Model.Issue.Metadata.Milestone ?? "No Milestone";
            _desc.Caption = Model.Issue.Content;
            _responsible.Caption = Model.Issue.Responsible != null ? Model.Issue.Responsible.Username : "Unassigned";
            if (Model.Issue.Responsible != null)
                _responsible.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            
            if (!string.IsNullOrEmpty(_desc.Caption))
            {
                _desc.Caption = _desc.Caption.Trim();
                if (_desc.Parent == null)
                {
                    InvokeOnMainThread(() => _details.Insert(0, _desc));
                }
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
                ReloadData();
                _comments.Clear();
                _comments.Insert(0,  UITableViewRowAnimation.None, comments);
                
                if (_scrollToLastComment && _comments.Elements.Count > 0)
                {
                    TableView.ScrollToRow(NSIndexPath.FromRowSection(_comments.Elements.Count - 1, 2), UITableViewScrollPosition.Top, true);
                    _scrollToLastComment = false;
                }
            });
        }
        
        protected override InternalIssueInfoModel OnUpdate (bool forced)
        {
            var l = Application.Client.Users[User].Repositories[Slug].Issues[Id];
            var m = new InternalIssueInfoModel() {
                Comments = l.Comments.GetComments(forced),
                Issue = l.GetIssue(forced),
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

