using System;
using CodeFramework.Elements;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using CodeBucket.Core.ViewModels;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.iOS.Utils;
using System.Linq;
using CodeFramework.iOS.Elements;

namespace CodeBucket.iOS.Views.Source
{
	public class ChangesetView : ViewModelDrivenDialogViewController
    {
        private readonly HeaderView _header = new HeaderView();
		private readonly UISegmentedControl _viewSegment;
		private readonly UIBarButtonItem _segmentBarButton;

        public new ChangesetViewModel ViewModel 
        {
            get { return (ChangesetViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
        }
        
        public ChangesetView()
        {
            Title = "Commit".t();
            Root.UnevenRows = true;
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
			_viewSegment = new UISegmentedControl(new string[] { "Changes".t(), "Comments".t(), "Approvals".t() });
			_viewSegment.SelectedSegment = 0;
			_viewSegment.ValueChanged += (sender, e) => Render();
			_segmentBarButton = new UIBarButtonItem(_viewSegment);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _header.Title = "Commit: ".t() + ViewModel.Node.Substring(0, ViewModel.Node.Length > 10 ? 10 : ViewModel.Node.Length);
            ViewModel.Bind(x => x.Commits, Render);
			ViewModel.BindCollection(x => x.Comments, a => Render());
			ViewModel.BindCollection(x => x.Participants, a => Render());
			_segmentBarButton.Width = View.Frame.Width - 10f;
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        public void Render()
        {
			if (ViewModel.Commits == null || ViewModel.Changeset == null)
				return;

            var commitModel = ViewModel.Commits;
            var root = new RootElement(Title) { UnevenRows = Root.UnevenRows };

			_header.Subtitle = "Commited ".t() + (ViewModel.Changeset.Utctimestamp).ToDaysAgo();
            var headerSection = new Section(_header);
            root.Add(headerSection);

            var detailSection = new Section();
            root.Add(detailSection);

            var user = "Unknown";
			if (ViewModel.Changeset.Author != null)
				user = ViewModel.Changeset.Author;

			detailSection.Add(new MultilinedElement(user, ViewModel.Changeset.Message)
            {
                CaptionColor = Theme.CurrentTheme.MainTextColor,
                ValueColor = Theme.CurrentTheme.MainTextColor,
                BackgroundColor = UIColor.White
            });

            if (ViewModel.ShowRepository)
            {
                var repo = new StyledStringElement(ViewModel.Repository) { 
                    Accessory = MonoTouch.UIKit.UITableViewCellAccessory.DisclosureIndicator, 
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
				foreach (var comment in ViewModel.Comments.Where(x => !x.Deleted && string.IsNullOrEmpty(x.Filename)))
				{
					commentSection.Add(new CommentElement
					{
						Name = comment.DisplayName,
						Time = comment.UtcCreatedOn.ToDaysAgo(),
						String = comment.Content,
						Image = Images.Anonymous,
						ImageUri = new Uri(comment.UserAvatarUrl),
						BackgroundColor = UIColor.White
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
				likeSection.AddAll(ViewModel.Participants.Where(x => x.Approved).Select(l => {
					var el = new UserElement(l.Username, string.Empty, string.Empty, l.Avatar);
					el.Tapped += () => ViewModel.GoToUserCommand.Execute(l.Username);
					return el;
				}));

				if (likeSection.Elements.Count > 0)
					root.Add(likeSection);

				StyledStringElement approveButton;
				if (ViewModel.Participants.Any(x => x.Username.Equals(ViewModel.GetApplication().Account.Username) && x.Approved))
				{
					approveButton = new StyledStringElement("Unapprove".t()) { Image = Images.Cancel };
					approveButton.Tapped += () => this.DoWorkAsync("Unapproving...", ViewModel.Unapprove);
				}
				else
				{
					approveButton = new StyledStringElement("Approve".t()) { Image = Images.Accept };
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
					await composer.DoWorkAsync("Commenting...".t(), () => ViewModel.AddComment(text));
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

			var sheet = MonoTouch.Utilities.GetSheet(Title);
			var addComment = sheet.AddButton("Add Comment".t());
			var copySha = sheet.AddButton("Copy Sha".t());
//			var shareButton = sheet.AddButton("Share".t());
			//var showButton = sheet.AddButton("Show in GitHub".t());
			var cancelButton = sheet.AddButton("Cancel".t());
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

