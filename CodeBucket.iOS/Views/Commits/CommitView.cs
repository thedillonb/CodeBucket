using System;
using CodeBucket.ViewControllers;
using UIKit;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using Humanizer;
using CodeBucket.Core.Utils;
using CodeBucket.DialogElements;
using System.Collections.Generic;
using CodeBucket.Utilities;
using CodeBucket.Services;

namespace CodeBucket.Views.Commits
{
    public class CommitView : PrettyDialogViewController
    {
		private readonly UISegmentedControl _viewSegment;
		private readonly UIBarButtonItem _segmentBarButton;

        public new CommitViewModel ViewModel 
        {
            get { return (CommitViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
        }
        
        public CommitView()
        {
			_viewSegment = new UISegmentedControl(new [] { "Changes", "Comments", "Approvals" });
			_viewSegment.SelectedSegment = 0;
			_viewSegment.ValueChanged += (sender, e) => Render();
			_segmentBarButton = new UIBarButtonItem(_viewSegment);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            Title = "Commit";

            HeaderView.SetImage(null, Images.Avatar);

            ViewModel.Bind(x => x.Commits).Subscribe(_ => Render());
            ViewModel.Bind(x => x.Commit).Subscribe(_ => Render());
            ViewModel.BindCollection(x => x.Comments).Subscribe(_ => Render());
			_segmentBarButton.Width = View.Frame.Width - 10f;
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            NavigationItem.RightBarButtonItem = null;
            ToolbarItems = null;
        }

        public void Render()
        {
			if (ViewModel.Commits == null || ViewModel.Commit == null)
				return;

            var titleMsg = (ViewModel.Commit.Message ?? string.Empty).Split(new [] { '\n' }, 2).FirstOrDefault();
            var avatarUrl = ViewModel.Commit.Author?.User?.Links?.Avatar?.Href;
            var node = ViewModel.Node.Substring(0, ViewModel.Node.Length > 10 ? 10 : ViewModel.Node.Length);

            Title = node;
            HeaderView.Text = titleMsg ?? node;
            HeaderView.SubText = "Commited " + (ViewModel.Commit.Date).Humanize();
            HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
            RefreshHeaderView();
     
            var split = new SplitButtonElement();
            split.AddButton("Comments", ViewModel.Comments.Items.Count.ToString());
            split.AddButton("Participants", ViewModel.Commit.Participants.Count.ToString());
            split.AddButton("Approvals", ViewModel.Commit.Participants.Count(x => x.Approved).ToString());

            var commitModel = ViewModel.Commits;
            ICollection<Section> root = new LinkedList<Section>();
            root.Add(new Section { split });

            var detailSection = new Section();
            root.Add(detailSection);

            var user = ViewModel.Commit.Author?.User?.DisplayName ?? ViewModel.Commit.Author.Raw ?? "Unknown";
            detailSection.Add(new MultilinedElement(user, ViewModel.Commit.Message));

            if (ViewModel.ShowRepository)
            {
                var repo = new StringElement(ViewModel.Repository) { 
                    Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    TextColor = StringElement.DefaultDetailColor,
                    Image = AtlassianIcon.Devtoolsrepository.ToImage()
                };
                repo.Clicked.BindCommand(ViewModel.GoToRepositoryCommand);
                detailSection.Add(repo);
            }

			if (_viewSegment.SelectedSegment == 0)
			{

				var paths = ViewModel.Commits.GroupBy(y =>
				{
					var filename = "/" + y.File;
					return filename.Substring(0, filename.LastIndexOf("/", System.StringComparison.Ordinal) + 1);
				}).OrderBy(y => y.Key);

				foreach (var p in paths)
				{
					var fileSection = new Section(p.Key);
					foreach (var x in p)
					{
						var y = x;
						var file = x.File.Substring(x.File.LastIndexOf('/') + 1);
						var sse = new ChangesetElement(file, x.Type, x.Diffstat.Added, x.Diffstat.Removed);
                        sse.Clicked.Select(_ => y).BindCommand(ViewModel.GoToFileCommand);
						fileSection.Add(sse);
					}
					root.Add(fileSection);
				}
			}
			else if (_viewSegment.SelectedSegment == 1)
			{
				var commentSection = new Section();
				foreach (var comment in ViewModel.Comments)
				{
                    var name = comment.User.DisplayName ?? comment.User.Username;
                    var avatar = new Avatar(comment.User.Links?.Avatar?.Href);
                    commentSection.Add(new CommentElement(name, comment.Content.Raw, comment.CreatedOn, avatar));
				}

				if (commentSection.Elements.Count > 0)
					root.Add(commentSection);

                var addComment = new StringElement("Add Comment") { Image = AtlassianIcon.Addcomment.ToImage() };
                addComment.Clicked.Subscribe(_ => AddCommentTapped());
				root.Add(new Section { addComment });
			}
			else if (_viewSegment.SelectedSegment == 2)
			{
				var likeSection = new Section();
                likeSection.AddAll(ViewModel.Commit.Participants.Where(x => x.Approved).Select(l => {
                    var avatar = new Avatar(l.User?.Links?.Avatar?.Href);
                    var el = new UserElement(l.User.DisplayName, string.Empty, string.Empty, avatar);
                    el.Clicked.Select(_ => l.User.Username).BindCommand(ViewModel.GoToUserCommand);
					return el;
				}));

				if (likeSection.Elements.Count > 0)
					root.Add(likeSection);

				StringElement approveButton;
                if (ViewModel.Commit.Participants.Any(x => x.User.Username.Equals(ViewModel.GetApplication().Account.Username) && x.Approved))
				{
                    approveButton = new StringElement("Unapprove") { Image = AtlassianIcon.Approve.ToImage() };
                    approveButton.Clicked.Subscribe(_ => this.DoWorkAsync("Unapproving...", ViewModel.Unapprove));
				}
				else
				{
                    approveButton = new StringElement("Approve") { Image = AtlassianIcon.Approve.ToImage() };
                    approveButton.Clicked.Subscribe(_ => this.DoWorkAsync("Approving...", ViewModel.Approve));
				}
				root.Add(new Section { approveButton });
			}

            Root.Reset(root); 
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
			composer.NewComment(this, async (text) => {
                try
                {
					await composer.DoWorkAsync("Commenting...", () => ViewModel.AddComment(text));
					composer.CloseComposer();
                }
                catch (Exception e)
                {
					AlertDialogService.ShowAlert("Unable to post comment!", e.Message);
                }
                finally
                {
                    composer.EnableSendButton = true;
                }
            });
        }

		private void ShowExtraMenu()
		{
			var changeset = ViewModel.Commits;
			if (changeset == null)
				return;

            var sheet = new UIActionSheet();
			var addComment = sheet.AddButton("Add Comment");
			var copySha = sheet.AddButton("Copy Sha");
//			var shareButton = sheet.AddButton("Share");
			//var showButton = sheet.AddButton("Show in GitHub");
			var cancelButton = sheet.AddButton("Cancel");
			sheet.CancelButtonIndex = cancelButton;
			sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {

                BeginInvokeOnMainThread(() =>
                {
    				// Pin to menu
    				if (e.ButtonIndex == addComment)
    				{
    					AddCommentTapped();
    				}
    				else if (e.ButtonIndex == copySha)
    				{
    					UIPasteboard.General.String = ViewModel.Node;
    				}
    //				else if (e.ButtonIndex == shareButton)
    //				{
    //					var item = UIActivity.FromObject (ViewModel.Changeset.Url);
    //					var activityItems = new MonoTouch.Foundation.NSObject[] { item };
    //					UIActivity[] applicationActivities = null;
    //					var activityController = new UIActivityViewController (activityItems, applicationActivities);
    //					PresentViewController (activityController, true, null);
    //				}
    //				else if (e.ButtonIndex == showButton)
    //				{
    //					ViewModel.GoToHtmlUrlCommand.Execute(null);
    //				}
                });

                sheet.Dispose();
			};

			sheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
		}

		public override void ViewWillAppear(bool animated)
		{
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };

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

