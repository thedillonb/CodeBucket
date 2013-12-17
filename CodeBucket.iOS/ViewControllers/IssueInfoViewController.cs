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

namespace CodeBucket.Bitbucket.Controllers.Issues
{
	public class IssueInfoViewController : BaseControllerDrivenViewController, IView<IssueInfoController.IssueInfoModel>
    {
        public int Id { get; private set; }
        public string User { get; private set; }
        public string Slug { get; private set; }

        private readonly HeaderView _header;
        private readonly Section _comments, _details;
        private readonly MultilinedElement _desc;
        private readonly SplitElement _split1, _split2, _split3;
        private readonly StyledStringElement _responsible;

        private bool _scrollToLastComment;
        private bool _issueRemoved;

        public new IssueInfoController Controller
        {
            get { return (IssueInfoController)base.Controller; }
            protected set { base.Controller = value; }
        }

        public IssueInfoViewController(string user, string slug, int id)
        {
            User = user;
            Slug = slug;
            Id = id;
            Title = "Issue #" + id;
            Controller = new IssueInfoController(this, user, slug, id);

            NavigationItem.RightBarButtonItem = new UIBarButtonItem(NavigationButton.Create(CodeFramework.Images.Buttons.Edit, () => {
                var m = Controller.Model;
                var editController = new IssueEditViewController {
                     ExistingIssue = m.Issue,
                     Username = User,
                     RepoSlug = Slug,
                     Title = "Edit Issue",
                     Success = EditingComplete,
                 };
                NavigationController.PushViewController(editController, true);
            }));
            NavigationItem.RightBarButtonItem.Enabled = false;

            Style = UITableViewStyle.Grouped;
            Root.UnevenRows = true;
            _header = new HeaderView(View.Bounds.Width) { ShadowImage = false };
            Root.Add(new Section(_header));

            _desc = new MultilinedElement("") { BackgroundColor = UIColor.White };
            _desc.CaptionFont = _desc.ValueFont;
            _desc.CaptionColor = _desc.ValueColor;

            _split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Buttons.Cog, Image2 = Images.Priority }) { BackgroundColor = UIColor.White };
            _split2 = new SplitElement(new SplitElement.Row { Image1 = Images.Buttons.Flag, Image2 = Images.ServerComponents }) { BackgroundColor = UIColor.White };
            _split3 = new SplitElement(new SplitElement.Row { Image1 = Images.SitemapColor, Image2 = Images.Milestone }) { BackgroundColor = UIColor.White };

            _responsible = new StyledStringElement("Unassigned") {
                Font = StyledStringElement.DefaultDetailFont,
                TextColor = StyledStringElement.DefaultDetailColor,
                Image = Images.Buttons.Person
            };
            _responsible.Tapped += () =>
            {
                var m = Controller.Model;
                if (m != null && m.Issue.Responsible != null)
                    NavigationController.PushViewController(new ProfileViewController(m.Issue.Responsible.Username), true);
            };

            var addComment = new StyledStringElement("Add Comment") { Image = Images.Pencil };
            addComment.Tapped += AddCommentTapped;

            _comments = new Section();
            _details = new Section { _split1, _split2, _split3, _responsible };

            Root.Add(_details);
            Root.Add(_comments);
            Root.Add(new Section { addComment });
        }

        public void Render(IssueInfoController.IssueInfoModel model)
        {
            //This means we've deleted it. Due to the code flow, render will get called after the update, regardless.
            if (model == null || model.Issue == null)
                return;

            NavigationItem.RightBarButtonItem.Enabled = true;
            _header.Title = model.Issue.Title;
            _header.Subtitle = "Updated " + (model.Issue.UtcLastUpdated).ToDaysAgo();
            _split1.Value.Text1 = model.Issue.Status;
            _split1.Value.Text2 = model.Issue.Priority;
            _split2.Value.Text1 = model.Issue.Metadata.Kind;
            _split2.Value.Text2 = model.Issue.Metadata.Component ?? "No Component";
            _split3.Value.Text1 = model.Issue.Metadata.Version ?? "No Version";
            _split3.Value.Text2 = model.Issue.Metadata.Milestone ?? "No Milestone";
            _desc.Caption = model.Issue.Content;
            _responsible.Caption = model.Issue.Responsible != null ? model.Issue.Responsible.Username : "Unassigned";
            if (model.Issue.Responsible != null)
                _responsible.Accessory = UITableViewCellAccessory.DisclosureIndicator;

            if (!string.IsNullOrEmpty(_desc.Caption))
            {
                _desc.Caption = _desc.Caption.Trim();
                if (_desc.Parent == null)
                {
                    InvokeOnMainThread(() => _details.Insert(0, _desc));
                }
            }

            var comments = new List<Element>(model.Comments.Count);
            model.Comments.OrderBy(x => (x.UtcCreatedOn)).ToList().ForEach(x =>
                                                                           {
                if (!string.IsNullOrEmpty(x.Content))
                    comments.Add(new CommentElement
                                 {
                        Name = x.AuthorInfo.Username,
                        Time = x.UtcCreatedOn.ToDaysAgo(),
                        String = x.Content,
                        Image = CodeFramework.Images.Misc.Anonymous,
                        ImageUri = new Uri(x.AuthorInfo.Avatar),
                        BackgroundColor = UIColor.White,
                    });
            });


            _header.SetNeedsDisplay();
            ReloadData();
            _comments.Clear();
            _comments.Insert(0, UITableViewRowAnimation.None, comments);

            if (_scrollToLastComment && _comments.Elements.Count > 0)
            {
                TableView.ScrollToRow(NSIndexPath.FromRowSection(_comments.Elements.Count - 1, 2), UITableViewScrollPosition.Top, true);
                _scrollToLastComment = false;
            }
        }

        void EditingComplete(IssueModel model)
        {
            Controller.EditComplete(model);

            //If it's null then we've deleted it!
            if (model == null)
                _issueRemoved = true;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_issueRemoved)
                NavigationController.PopViewControllerAnimated(true);
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
            composer.NewComment(this, () => {
                var text = composer.Text;
                composer.DoWork(() => {
                    Controller.AddComment(text);
                    InvokeOnMainThread(() => {
                        composer.CloseComposer();
                        _scrollToLastComment = true;
                    });
                }, ex =>
                {
                    Utilities.ShowAlert("Unable to post comment!", ex.Message);
                    composer.EnableSendButton = true;
                });
            });
        }

        public override UIView InputAccessoryView
        {
            get
            {
                var u = new UIView(new RectangleF(0, 0, 320f, 27)) { BackgroundColor = UIColor.White };
                return u;
            }
        }
    }
}

