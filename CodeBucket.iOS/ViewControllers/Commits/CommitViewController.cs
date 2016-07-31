using System;
using UIKit;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Commits;
using Humanizer;
using CodeBucket.Core.Utils;
using CodeBucket.DialogElements;
using ReactiveUI;
using CodeBucket.ViewControllers.Comments;
using CodeBucket.Views;
using CodeBucket.Client;

namespace CodeBucket.ViewControllers.Commits
{
    public class CommitViewController : PrettyDialogViewController<CommitViewModel>
    {
        private HtmlElement _commentsElement = new HtmlElement("comments");

        public CommitViewController()
        {
            OnActivation(d =>
            {
                _commentsElement
                    .UrlRequested
                    .Select(x => new WebBrowserViewController(x))
                    .Subscribe(x => PresentViewController(x, true, null))
                    .AddTo(d);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.SetImage(null, Images.Avatar);

            var detailsSection = new Section();
            var diffButton = new ButtonElement(AtlassianIcon.ListAdd.ToImage());
            var removedButton = new ButtonElement(AtlassianIcon.ListRemove.ToImage());
            var modifiedButton = new ButtonElement(AtlassianIcon.Edit.ToImage());
            var allChangesButton = new ButtonElement("All Changes", AtlassianIcon.Devtoolssidediff.ToImage());
            detailsSection.Add(new [] { diffButton, removedButton, modifiedButton, allChangesButton });

            var additions = ViewModel.WhenAnyValue(x => x.DiffAdditions);
            diffButton.BindClick(ViewModel.GoToAddedFiles);
            diffButton.BindDisclosure(additions.Select(x => x > 0));
            diffButton.BindCaption(additions.Select(x => $"{x} Added"));

            var deletions = ViewModel.WhenAnyValue(x => x.DiffDeletions);
            removedButton.BindClick(ViewModel.GoToRemovedFiles);
            removedButton.BindDisclosure(deletions.Select(x => x > 0));
            removedButton.BindCaption(deletions.Select(x => $"{x} Removed"));

            var modifications = ViewModel.WhenAnyValue(x => x.DiffModifications);
            modifiedButton.BindClick(ViewModel.GoToModifiedFiles);
            modifiedButton.BindDisclosure(modifications.Select(x => x > 0));
            modifiedButton.BindCaption(modifications.Select(x => $"{x} Modified"));

            allChangesButton.BindClick(ViewModel.GoToAllFiles);

            var split = new SplitButtonElement();
            var commentCount = split.AddButton("Comments", "-");
            var participants = split.AddButton("Participants", "-");
            var approvals = split.AddButton("Approvals", "-");
            var headerSection = new Section { split };

            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;

            var detailSection = new Section();
            var detailsElement = new MultilinedElement();
            detailSection.Add(detailsElement);

            var approvalSection = new Section("Approvals");
            var approveElement = new LoaderButtonElement("Approve", AtlassianIcon.Approve.ToImage());
            approveElement.Accessory = UITableViewCellAccessory.None;
            approveElement.BindLoader(ViewModel.ToggleApproveButton);
            approveElement.BindCaption(ViewModel.WhenAnyValue(x => x.Approved).Select(x => x ? "Unapprove" : "Approve"));

            var commentsSection = new Section("Comments");
            var addCommentElement = new ButtonElement("Add Comment", AtlassianIcon.Addcomment.ToImage());
            commentsSection.Add(addCommentElement);

            if (ViewModel.ShowRepository)
            {
                var repo = new ButtonElement(ViewModel.Repository) { 
                    TextColor = StringElement.DefaultDetailColor,
                    Image = AtlassianIcon.Devtoolsrepository.ToImage()
                };

                detailSection.Add(repo);
                repo.Clicked.BindCommand(ViewModel.GoToRepositoryCommand);
            }

            ViewModel.WhenAnyValue(x => x.Commit).IsNotNull().Subscribe(x => {
                participants.Text = x?.Participants?.Count.ToString() ?? "-";
                approvals.Text = x?.Participants?.Count(y => y.Approved).ToString() ?? "-";

                var titleMsg = (ViewModel.Commit.Message ?? string.Empty).Split(new [] { '\n' }, 2).FirstOrDefault();
                var avatarUrl = ViewModel.Commit.Author?.User?.Links?.Avatar?.Href;

                HeaderView.SubText = titleMsg ?? "Commited " + (ViewModel.Commit.Date).Humanize();
                HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
                RefreshHeaderView();

                var user = x.Author?.User?.DisplayName ?? x.Author?.Raw ?? "Unknown";
                detailsElement.Caption = user;
                detailsElement.Details = x.Message;
            });

            this.WhenAnyValue(x => x.ViewModel.Approvals)
                .Select(x => x.Select(y => new UserElement(y)).OfType<Element>())
                .Subscribe(x => approvalSection.Reset(x.Concat(new[] { approveElement })));

            ViewModel.Comments
                     .CountChanged
                     .Select(x => x.ToString())
                     .StartWith("-")
                     .Subscribe(x => commentCount.Text = x);

            ViewModel
                .Comments
                .ChangedObservable()
                .Subscribe(x =>
                {
                    if (x.Count > 0)
                    {
                        var comments = x.Select(y => new Comment(y.Avatar.ToUrl(), y.Name, y.Content, y.CreatedOn)).ToList();
                        var commentModel = new Views.CommentModel(comments, (int)UIFont.PreferredSubheadline.PointSize);
                        var content = new CommentsView { Model = commentModel }.GenerateString();
                        _commentsElement.SetValue(content);
                        commentsSection.Insert(0, UITableViewRowAnimation.None, _commentsElement);
                    }
                    else
                    {
                        commentsSection.Remove(_commentsElement);
                    }
                });

            ViewModel.WhenAnyValue(x => x.Commit)
                .IsNotNull()
                .Take(1)
                .Subscribe(_ => Root.Reset(headerSection, detailsSection, approvalSection, commentsSection));

            ViewModel.GoToAddedFiles.Select(_ => CommitFileType.Added).Subscribe(GoToFiles);
            ViewModel.GoToRemovedFiles.Select(_ => CommitFileType.Removed).Subscribe(GoToFiles);
            ViewModel.GoToModifiedFiles.Select(_ => CommitFileType.Modified).Subscribe(GoToFiles);
            ViewModel.GoToAllFiles.Subscribe(_ => GoToAllFiles());

            OnActivation(d =>
            {
                actionButton
                    .GetClickedObservable()
                    .InvokeCommand(ViewModel.ShowMenuCommand)
                    .AddTo(d);

                addCommentElement
                    .Clicked
                    .InvokeCommand(ViewModel.AddCommentCommand)
                    .AddTo(d);

                this.WhenAnyObservable(x => x.ViewModel.AddCommentCommand)
                    .Subscribe(_ => NewCommentViewController.Present(this, ViewModel.AddComment))
                    .AddTo(d);
            });
        }

        private void GoToFiles(CommitFileType commitFileType)
        {
            var files = ViewModel.CommitFiles.Where(x => x.Type == commitFileType);
            var viewController = new CommitFileChangesViewController(files);
            viewController.Title = commitFileType.Humanize(LetterCasing.Title);
            NavigationController.PushViewController(viewController, true);
        }

        private void GoToAllFiles()
        {
            var viewController = new CommitFileChangesViewController(ViewModel.CommitFiles);
            viewController.Title = "All Changes";
            NavigationController.PushViewController(viewController, true);
        }
    }
}

