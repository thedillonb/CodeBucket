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

namespace CodeBucket.ViewControllers
{
    public class ChangesetInfoViewController : BaseControllerDrivenViewController, IView<ChangesetInfoController.ChangesetInfoModel>
    {
        public string Node { get; private set; }
        
        public string User { get; private set; }
        
        public string Slug { get; private set; }
        
        public RepositoryDetailedModel Repo { get; set; }

        public new ChangesetInfoController Controller 
        {
            get { return (ChangesetInfoController)base.Controller; }
            protected set { base.Controller = value; }
        }
        
        private readonly HeaderView _header;
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;
        
        public ChangesetInfoViewController(string user, string slug, string node)
        {
            Node = node;
            User = user;
            Slug = slug;
            Title = "Commit".t();
            Root.UnevenRows = true;
            Controller = new ChangesetInfoController(this, user, slug, node);
            
            _header = new HeaderView(0f) { Title = "Commit: ".t() + node.Substring(0, node.Length > 10 ? 10 : node.Length) };
            _viewSegment = new UISegmentedControl(new string[] { "Changes".t(), "Comments".t(), "Approvals".t() });
            _viewSegment.ControlStyle = UISegmentedControlStyle.Bar;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Fucking bug in the divider
            BeginInvokeOnMainThread(delegate {
                _viewSegment.SelectedSegment = 1;
                _viewSegment.SelectedSegment = 0;
                _viewSegment.ValueChanged += (sender, e) => Controller.Render();
            });

            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        public void Render(ChangesetInfoController.ChangesetInfoModel model)
        {
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

            _header.Subtitle = "Commited ".t() + (model.Changeset.Utctimestamp).ToDaysAgo();
            var headerSection = new Section(_header);
            root.Add(headerSection);

            var detailSection = new Section();
            root.Add(detailSection);

            var d = new MultilinedElement(model.Changeset.Author, model.Changeset.Message);
            detailSection.Add(d);

            if (Repo != null)
            {
                var repo = new StyledStringElement(Repo.Name) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    Font = StyledStringElement.DefaultDetailFont, 
                    TextColor = StyledStringElement.DefaultDetailColor,
                    Image = Images.Repo
                };
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoViewController(Repo), true);
                detailSection.Add(repo);
            }

            if (_viewSegment.SelectedSegment == 0)
            {
                var fileSection = new Section();
                model.Changeset.Files.ForEach(x => 
                                              {
                    int? added = null;
                    int? removed = null;

                    if (model.Diffs.ContainsKey(x.File))
                    {
                        var diff = model.Diffs[x.File];
                        added = diff.Diffstat.Added;
                        removed = diff.Diffstat.Removed;
                    }

                    var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
                    var sse = new ChangesetElement(file, x.Type, added, removed);
                    sse.Tapped += () => {
                        string parent = null;
                        if (model.Changeset.Parents != null && model.Changeset.Parents.Count > 0)
                            parent = model.Changeset.Parents[0];

                        var type = x.Type.Trim().ToLower();
                        NavigationController.PushViewController(new ChangesetDiffViewController(User, Slug, model.Changeset.Node, parent, x.File) { 
                            Removed = type.Equals("removed"), Added = type.Equals("added"), Comments = Controller.Model.Comments
                        }, true);
                    };
                    fileSection.Add(sse);
                });

                if (fileSection.Elements.Count > 0)
                    root.Add(fileSection);
            }
            else if (_viewSegment.SelectedSegment == 1)
            {
                var commentSection = new Section();
                foreach (var comment in model.Comments)
                {
                    if (comment.Deleted || !string.IsNullOrEmpty(comment.Filename))
                        continue;

                    commentSection.Add(new CommentElement {
                        Name = comment.DisplayName,
                        Time = comment.UtcCreatedOn.ToDaysAgo(),
                        String = comment.Content,
                        Image = CodeFramework.Images.Misc.Anonymous,
                        ImageUri = new Uri(comment.UserAvatarUrl),
                        BackgroundColor = UIColor.White,
                    });
                }

                if (commentSection.Elements.Count > 0)
                    root.Add(commentSection);

                var addComment = new StyledStringElement("Add Comment".t()) { Image = Images.Pencil };
                addComment.Tapped += AddCommentTapped;
                root.Add(new Section { addComment });
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                var likeSection = new Section();
                likeSection.AddAll(model.Likes.Where(x => x.Approved).Select(l => {
                    var el = new UserElement(l.Username, l.FirstName, l.LastName, l.Avatar);
                    el.Tapped += () => NavigationController.PushViewController(new ProfileViewController(l.Username), true);
                    return el;
                }));

                if (likeSection.Elements.Count > 0)
                    root.Add(likeSection);

                StyledStringElement approveButton;
                if (model.Likes.Exists(x => x.Username.Equals(Application.Account.Username) && x.Approved))
                {
                    approveButton = new StyledStringElement("Unapprove".t()) { Image = Images.Cancel };
                    approveButton.Tapped += UnApprovedTapped;
                }
                else
                {
                    approveButton = new StyledStringElement("Approve".t()) { Image = Images.Accept };
                    approveButton.Tapped += ApproveTapped;
                }
                root.Add(new Section { approveButton });
            }

            Root = root; 
        }

        void ApproveTapped()
        {
            this.DoWork("Approving...".t(), () => Controller.Approve(), ex => {
                Utilities.ShowAlert("Unable to approve changeset!".t(), ex.Message);
            });
        }

        void UnApprovedTapped()
        {
            this.DoWork("Unapproving...".t(), () => Controller.Unapprove(), ex => {
                Utilities.ShowAlert("Unable to unapprove changeset!".t(), ex.Message);
            });
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
            composer.NewComment(this, () => {
                var text = composer.Text;
                composer.DoWork(() => {
                    Controller.AddComment(text);
                    InvokeOnMainThread(() => composer.CloseComposer());
                }, ex => {
                    Utilities.ShowAlert("Unable to post comment!", ex.Message);
                    composer.EnableSendButton = true;
                });
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

