using System;
using CodeBucket.ViewControllers;
using UIKit;
using CodeBucket.Utils;
using System.Linq;
using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Commits;
using Humanizer;
using CodeBucket.Core.Utils;

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

            Title = "Commit";
            Root.UnevenRows = true;
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());

            HeaderView.SetImage(null, Images.Avatar);

            ViewModel.Bind(x => x.Commits, Render);
            ViewModel.Bind(x => x.Commit, Render);
			ViewModel.BindCollection(x => x.Comments, a => Render());
			_segmentBarButton.Width = View.Frame.Width - 10f;
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
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
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };
            root.Add(new Section { split });

            var detailSection = new Section();
            root.Add(detailSection);

            var user = ViewModel.Commit.Author?.User?.DisplayName ?? ViewModel.Commit.Author.Raw ?? "Unknown";
			detailSection.Add(new MultilinedElement(user, ViewModel.Commit.Message)
            {
                CaptionColor = Theme.CurrentTheme.MainTextColor,
                ValueColor = Theme.CurrentTheme.MainTextColor,
                BackgroundColor = UIColor.White
            });

            if (ViewModel.ShowRepository)
            {
                var repo = new StyledStringElement(ViewModel.Repository) { 
                    Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    Font = StyledStringElement.DefaultDetailFont, 
                    TextColor = StyledStringElement.DefaultDetailColor,
                    Image = Images.Repo
                };
                repo.Tapped += () => ViewModel.GoToRepositoryCommand.Execute(null);
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
						sse.Tapped += () => ViewModel.GoToFileCommand.Execute(y);
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
                    var imgUri = new Avatar(comment.User.Links?.Avatar?.Href);
                    commentSection.Add(new NameTimeStringElement(name, comment.Content.Raw, comment.CreatedOn, imgUri.ToUrl(), Images.Avatar));
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
                likeSection.AddAll(ViewModel.Commit.Participants.Where(x => x.Approved).Select(l => {
                    var el = new UserElement(l.User.DisplayName, string.Empty, string.Empty, l.User.Links.Avatar.Href);
                    el.Tapped += () => ViewModel.GoToUserCommand.Execute(l.User.Username);
					return el;
				}));

				if (likeSection.Elements.Count > 0)
					root.Add(likeSection);

				StyledStringElement approveButton;
                if (ViewModel.Commit.Participants.Any(x => x.User.Username.Equals(ViewModel.GetApplication().Account.Username) && x.Approved))
				{
					approveButton = new StyledStringElement("Unapprove") { Image = Images.Cancel };
					approveButton.Tapped += () => this.DoWorkAsync("Unapproving...", ViewModel.Unapprove);
				}
				else
				{
					approveButton = new StyledStringElement("Approve") { Image = Images.Accept };
					approveButton.Tapped += () => this.DoWorkAsync("Approving...", ViewModel.Approve);
				}
				root.Add(new Section { approveButton });
			}

			Root = root; 
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
					MonoTouch.Utilities.ShowAlert("Unable to post comment!", e.Message);
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

			var sheet = MonoTouch.Utilities.GetSheet();
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
			};

			sheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
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

