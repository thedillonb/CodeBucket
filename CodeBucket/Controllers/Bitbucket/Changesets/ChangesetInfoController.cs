using System;
using CodeBucket.Bitbucket.Controllers;
using BitbucketSharp.Models;
using MonoTouch.Dialog;
using CodeBucket.Bitbucket.Controllers.Repositories;
using System.Linq;
using CodeBucket.Controllers;
using CodeFramework.Controllers;
using CodeFramework.Views;
using CodeFramework.Elements;
using System.Collections.Generic;
using MonoTouch.UIKit;
using MonoTouch;
using MonoTouch.Foundation;

namespace CodeBucket.Bitbucket.Controllers.Changesets
{
    public class ChangesetInfoController : BaseModelDrivenController
    {
        public string Node { get; private set; }
        
        public string User { get; private set; }
        
        public string Slug { get; private set; }
        
        public RepositoryDetailedModel Repo { get; set; }
        
        private readonly HeaderView _header;
        private readonly UISegmentedControl _viewSegment;
        private readonly UIBarButtonItem _segmentBarButton;
        
        public ChangesetInfoController(string user, string slug, string node)
            : base(typeof(InnerChangesetModel))
        {
            Node = node;
            User = user;
            Slug = slug;
            Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
            Title = "Commit";
            Root.UnevenRows = true;
            
            _header = new HeaderView(0f) { Title = "Commit: " + node.Substring(0, node.Length > 10 ? 10 : node.Length) };
            _viewSegment = new UISegmentedControl(new string[] { "Changes", "Comments", "Approvals" });
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
                _viewSegment.ValueChanged += (sender, e) => Render();
            });

            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        protected override void OnRender()
        {
            var model = (InnerChangesetModel)Model;
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

            _header.Subtitle = "Commited " + (model.Changeset.Utctimestamp).ToDaysAgo();
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
                repo.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(Repo), true);
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
                        NavigationController.PushViewController(new ChangesetDiffController(User, Slug, model.Changeset.Node, parent, x.File) { 
                            Removed = type.Equals("removed"), Added = type.Equals("added") 
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

                var addComment = new StyledStringElement("Add Comment") { Image = Images.Pencil };
                addComment.Tapped += AddCommentTapped;
                root.Add(new Section { addComment });
            }
            else if (_viewSegment.SelectedSegment == 2)
            {
                var likeSection = new Section();
                foreach (var l in model.Likes)
                {
                    if (l.Approved)
                        likeSection.Add(new UserElement(l.Username, l.FirstName, l.LastName, l.Avatar));
                }

                if (likeSection.Elements.Count > 0)
                    root.Add(likeSection);

                StyledStringElement approveButton;
                if (model.Likes.Exists(x => x.Username.Equals(Application.Account.Username) && x.Approved))
                {
                    approveButton = new StyledStringElement("Unapprove") { Image = Images.Cancel };
                    approveButton.Tapped += UnApprovedTapped;
                }
                else
                {
                    approveButton = new StyledStringElement("Approve") { Image = Images.Accept };
                    approveButton.Tapped += ApproveTapped;
                }
                root.Add(new Section { approveButton });
            }

            Root = root; 
        }

        void ApproveTapped()
        {
            this.DoWork("Approving...", () => {
                Application.Client.Users[User].Repositories[Slug].Changesets[Node].Approve();
                var model = (InnerChangesetModel)Model;
                model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
                BeginInvokeOnMainThread(() => Render());
            }, ex => {
                Utilities.ShowAlert("Unable to approve changeset!", ex.Message);
            });
        }

        void UnApprovedTapped()
        {
            this.DoWork("Unapproving...", () => {
                Application.Client.Users[User].Repositories[Slug].Changesets[Node].Unapprove();
                var model = (InnerChangesetModel)Model;
                model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(true);
                BeginInvokeOnMainThread(() => Render());
            }, ex => {
                Utilities.ShowAlert("Unable to unapprove changeset!", ex.Message);
            });
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
            composer.NewComment(this, () => {
                var comment = new CreateChangesetCommentModel { Content = composer.Text };

                composer.DoWork(() => {
                    var c = Application.Client.Users[User].Repositories[Slug].Changesets[Node].Comments.Create(comment);

                    InvokeOnMainThread(() => {
                        composer.CloseComposer();
                        var model = (InnerChangesetModel)Model;
                        model.Comments.Add(c);
                        Render();
                    });
                }, ex =>
                {
                    Utilities.ShowAlert("Unable to post comment!", ex.Message);
                    composer.EnableSendButton = true;
                });
            });
        }

        protected override object OnUpdateModel(bool forced)
        {
            var model = new InnerChangesetModel();
            var x = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetInfo(forced);
            x.Files = x.Files.OrderBy(y => y.File.Substring(y.File.LastIndexOf('/') + 1)).ToList();
            model.Changeset = x;

            // Try to get these things
            try
            {
                model.Diffs = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetDiffs(forced).ToDictionary(e => e.File);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get diffs", e);
            }

            try
            {
                model.Comments = Application.Client.Users[User].Repositories[Slug].Changesets[Node].Comments.GetComments(forced);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get comments", e);
            }

            try
            {
                model.Likes = Application.Client.Users[User].Repositories[Slug].Changesets[Node].GetParticipants(forced);
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.LogException("Unable to get likes", e);
            }

            return model;
        }

        /// <summary>
        /// An inner class that combines two external models
        /// </summary>
        private class InnerChangesetModel
        {
            public ChangesetModel Changeset { get; set; }
            public Dictionary<string, ChangesetDiffModel> Diffs { get; set; }
            public List<ChangesetCommentModel> Comments { get; set; }
            public List<ChangesetParticipantsModel> Likes { get; set; }

            public InnerChangesetModel()
            {
                Diffs = new Dictionary<string, ChangesetDiffModel>();
                Comments = new List<ChangesetCommentModel>();
                Likes = new List<ChangesetParticipantsModel>();
            }
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

