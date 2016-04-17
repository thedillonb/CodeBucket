using System;
using UIKit;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using Humanizer;
using CodeBucket.Core.Utils;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;
using CodeBucket.Services;
using BitbucketSharp.Models.V2;
using BitbucketSharp.Models;

namespace CodeBucket.ViewControllers.Commits
{
    public class CommitViewController : PrettyDialogViewController
    {
        public new CommitViewModel ViewModel 
        {
            get { return (CommitViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            Title = "Commit";

            HeaderView.SetImage(null, Images.Avatar);

            var detailsSection = new Section();
            var diffButton = new StringElement(AtlassianIcon.ListAdd.ToImage());
            var removedButton = new StringElement(AtlassianIcon.ListRemove.ToImage());
            var modifiedButton = new StringElement(AtlassianIcon.Edit.ToImage());
            var allChangesButton = new StringElement("All Changes", AtlassianIcon.Devtoolssidediff.ToImage());
            detailsSection.Add(new [] { diffButton, removedButton, modifiedButton, allChangesButton });

            var additions = ViewModel.Bind(x => x.DiffAdditions, true);
            diffButton.BindClick(ViewModel.GoToAddedFiles);
            diffButton.BindDisclosure(additions.Select(x => x > 0));
            diffButton.BindCaption(additions.Select(x => $"{x} Added"));

            var deletions = ViewModel.Bind(x => x.DiffDeletions, true);
            removedButton.BindClick(ViewModel.GoToRemovedFiles);
            removedButton.BindDisclosure(deletions.Select(x => x > 0));
            removedButton.BindCaption(deletions.Select(x => $"{x} Removed"));

            var modifications = ViewModel.Bind(x => x.DiffModifications, true);
            modifiedButton.BindClick(ViewModel.GoToModifiedFiles);
            modifiedButton.BindDisclosure(modifications.Select(x => x > 0));
            modifiedButton.BindCaption(modifications.Select(x => $"{x} Modified"));

            allChangesButton.BindClick(ViewModel.GoToAllFiles);

            var split = new SplitButtonElement();
            var comments = split.AddButton("Comments", "-");
            var participants = split.AddButton("Participants", "-");
            var approvals = split.AddButton("Approvals", "-");
            var headerSection = new Section { split };

            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;
            OnActivation(d => d(actionButton.BindCommand(ViewModel.ShowMenuCommand)));

            var detailSection = new Section();
            var detailsElement = new MultilinedElement();
            detailSection.Add(detailsElement);

            var approvalSection = new Section("Approvals");
            var approveElement = new LoaderButtonElement("Approve", AtlassianIcon.Approve.ToImage());
            approveElement.Accessory = UITableViewCellAccessory.None;
            approveElement.BindLoader(ViewModel.ToggleApproveButton);
            approveElement.BindCaption(ViewModel.Bind(x => x.Approved, true).Select(x => x ? "Unapprove" : "Approve"));

            var commentsSection = new Section("Comments");
            var addCommentElement = new StringElement("Add Comment", AtlassianIcon.Addcomment.ToImage());
            addCommentElement.Clicked.Subscribe(_ => AddCommentTapped());

            if (ViewModel.ShowRepository)
            {
                var repo = new StringElement(ViewModel.Repository) { 
                    Accessory = UITableViewCellAccessory.DisclosureIndicator, 
                    Lines = 1, 
                    TextColor = StringElement.DefaultDetailColor,
                    Image = AtlassianIcon.Devtoolsrepository.ToImage()
                };

                detailSection.Add(repo);
                repo.Clicked.BindCommand(ViewModel.GoToRepositoryCommand);
            }

            ViewModel.Bind(x => x.Commit, true).IsNotNull().Subscribe(x => {
                participants.Text = x?.Participants.Count.ToString() ?? "-";
                approvals.Text = x?.Participants.Count(y => y.Approved).ToString() ?? "-";

                var titleMsg = (ViewModel.Commit.Message ?? string.Empty).Split(new [] { '\n' }, 2).FirstOrDefault();
                var avatarUrl = ViewModel.Commit.Author?.User?.Links?.Avatar?.Href;
                var node = ViewModel.Node.Substring(0, ViewModel.Node.Length > 10 ? 10 : ViewModel.Node.Length);

                Title = node;
                HeaderView.Text = titleMsg ?? node;
                HeaderView.SubText = "Commited " + (ViewModel.Commit.Date).Humanize();
                HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
                RefreshHeaderView();

                var user = x.Author?.User?.DisplayName ?? x.Author.Raw ?? "Unknown";
                detailsElement.Caption = user;
                detailsElement.Details = x.Message;

                var participantElements = (x.Participants ?? Enumerable.Empty<Participant>())
                    .Where(y => y.Approved)
                    .Select(l => {
                        var avatar = new Avatar(l.User?.Links?.Avatar?.Href);
                        var el = new UserElement(l.User.DisplayName, string.Empty, string.Empty, avatar);
                        el.Clicked.Select(_ => l.User.Username).BindCommand(ViewModel.GoToUserCommand);
                        return el;
                    })
                    .OfType<Element>();

                approvalSection.Reset(participantElements.Concat(new [] { approveElement }));
            });

            ViewModel.Bind(x => x.Comments, true).Subscribe(x => {
                comments.Text = x?.Count.ToString() ?? "-";

                var commentElements = (x ?? Enumerable.Empty<CommitComment>())
                    .Select(comment => {
                        var name = comment.User.DisplayName ?? comment.User.Username;
                        var avatar = new Avatar(comment.User.Links?.Avatar?.Href);
                        return new CommentElement(name, comment.Content.Raw, comment.CreatedOn, avatar);
                    })
                    .OfType<Element>();

                commentsSection.Reset(commentElements.Concat(new [] { addCommentElement }));
            });

            ViewModel.Bind(x => x.Commit, true)
                .IsNotNull()
                .Take(1)
                .Subscribe(_ => Root.Reset(headerSection, detailsSection, approvalSection, commentsSection));

            ViewModel.GoToAddedFiles.Select(_ => ChangesetModel.FileType.Added).Subscribe(GoToFiles);
            ViewModel.GoToRemovedFiles.Select(_ => ChangesetModel.FileType.Removed).Subscribe(GoToFiles);
            ViewModel.GoToModifiedFiles.Select(_ => ChangesetModel.FileType.Modified).Subscribe(GoToFiles);
            ViewModel.GoToAllFiles.Subscribe(_ => GoToAllFiles());
        }

        private void GoToFiles(ChangesetModel.FileType commitFileType)
        {
            var files = ViewModel.CommitFiles.Where(x => x.Type == commitFileType);
            var viewController = new CommitFileChangesViewController(files);
            viewController.Title = commitFileType.ToString();
            NavigationController.PushViewController(viewController, true);
        }

        private void GoToAllFiles()
        {
            var viewController = new CommitFileChangesViewController(ViewModel.CommitFiles);
            viewController.Title = "All Changes";
            NavigationController.PushViewController(viewController, true);
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
    }
}

