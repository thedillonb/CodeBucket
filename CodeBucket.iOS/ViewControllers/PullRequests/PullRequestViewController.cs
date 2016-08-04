using System;
using System.Linq;
using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.DialogElements;
using Humanizer;
using CodeBucket.Core.Utils;
using System.Collections.Generic;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Views;
using CodeBucket.ViewControllers.Comments;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestViewController : PrettyDialogViewController<PullRequestViewModel>
    {
        private readonly HtmlElement _descriptionElement = new HtmlElement("description");
        private readonly HtmlElement _commentsElement = new HtmlElement("comments");

        public PullRequestViewController()
        {
            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;

            OnActivation(d =>
            {
                Observable.Merge(_descriptionElement.UrlRequested, _commentsElement.UrlRequested)
                    .Select(x => new WebBrowserViewController(x))
                    .Subscribe(x => PresentViewController(x, true, null))
                    .AddTo(d);

                actionButton.Bind(ViewModel.ShowMenuCommand).AddTo(d);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var split = new SplitButtonElement();
            var commentCount = split.AddButton("Comments", "-");
            var participants = split.AddButton("Participants", "-");
            var approvals = split.AddButton("Approvals", "-");

            this.WhenAnyValue(x => x.ViewModel.Title)
                .Subscribe(x => RefreshHeaderView(x));

            ViewModel.WhenAnyValue(x => x.PullRequest).Subscribe(x =>
            {
                if (x != null)
                {
                    var avatarUrl = x?.Author?.Links?.Avatar?.Href;
                    HeaderView.SubText = "Updated " + ViewModel.PullRequest.UpdatedOn.Humanize();
                    HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
                }
                else
                {
                    HeaderView.SetImage(null, Images.Avatar);
                    HeaderView.SubText = null;
                }
            });

            var root = new List<Section>();
            root.Add(new Section { split });

            var secDetails = new Section();
            root.Add(secDetails);

            this.WhenAnyValue(x => x.ViewModel.Description)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize, true))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(content =>
                {
                    _descriptionElement.SetValue(content);
                    secDetails.Insert(0, UITableViewRowAnimation.None, _descriptionElement);
                });

            var split1 = new SplitViewElement(AtlassianIcon.Calendar.ToImage(), AtlassianIcon.Devtoolsbranch.ToImage());
            secDetails.Add(split1);

            this.WhenAnyValue(x => x.ViewModel.PullRequest.State)
                .Select(x => x?.ToLower().Humanize(LetterCasing.Title))
                .Subscribe(x => split1.Button2.Text = x ?? "Open");

            this.WhenAnyValue(x => x.ViewModel.PullRequest)
                .Select(x => x?.CreatedOn.ToString("MM/dd/yy"))
                .Subscribe(x => split1.Button1.Text = x);

            var commitsElement = new ButtonElement("Commits", AtlassianIcon.Devtoolscommit.ToImage());
            commitsElement.Clicked.BindCommand(ViewModel.GoToCommitsCommand);
            secDetails.Add(commitsElement);

            var mergeElement = new ButtonElement("Merge", AtlassianIcon.ListAdd.ToImage());
            mergeElement.Accessory = UITableViewCellAccessory.None;

            var rejectElement = new LoaderButtonElement("Reject", AtlassianIcon.ListRemove.ToImage());
            rejectElement.Accessory = UITableViewCellAccessory.None;
            rejectElement.BindLoader(ViewModel.RejectCommand);

            var mergeSection = new Section { mergeElement, rejectElement };

            var approvalSection = new Section("Approvals");
            var approveElement = new LoaderButtonElement("Approve", AtlassianIcon.Approve.ToImage());
            approveElement.Accessory = UITableViewCellAccessory.None;
            approveElement.BindLoader(ViewModel.ToggleApproveButton);
            approveElement.BindCaption(this.WhenAnyValue(x => x.ViewModel.Approved).Select(x => x ? "Unapprove" : "Approve"));
            root.Add(approvalSection);

            this.WhenAnyValue(x => x.ViewModel.Approvals)
                .Select(x => x.Select(y => new UserElement(y)).OfType<Element>())
                .Subscribe(x => approvalSection.Reset(x.Concat(new[] { approveElement })));

            var commentsSection = new Section("Comments");
            root.Add(commentsSection);

            var addComment = new ButtonElement("Add Comment") { Image = AtlassianIcon.Addcomment.ToImage() };
            commentsSection.Reset(new[] { addComment });

            ViewModel
                .Comments
                .ChangedObservable()
                .Subscribe(x =>
                {
                    if (x.Count > 0)
                    {
                        var comments = x.Select(y => new Comment(y.Avatar.ToUrl(), y.Name, y.Content, y.CreatedOn)).ToList();
                        var commentModel = new CommentModel(comments, (int)UIFont.PreferredSubheadline.PointSize);
                        var content = new CommentsView { Model = commentModel }.GenerateString();
                        _commentsElement.SetValue(content);
                        commentsSection.Insert(0, UITableViewRowAnimation.None, _commentsElement);
                    }
                    else
                    {
                        commentsSection.Remove(_commentsElement);
                    }
                });

            Root.Reset(root);

            this.WhenAnyValue(x => x.ViewModel.IsOpen).Subscribe(x =>
            {
                if (x)
                    Root.Insert(root.IndexOf(secDetails) + 1, mergeSection);
                else
                    Root.Remove(mergeSection);
            });

            OnActivation(disposable =>
            {
                mergeElement
                    .Clicked
                    .InvokeCommand(this, x => x.ViewModel.MergeCommand)
                    .AddTo(disposable);

                addComment
                    .Clicked
                    .Subscribe(_ => NewCommentViewController.Present(this, ViewModel.AddComment))
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.ParticipantCount)
                    .Subscribe(x => participants.Text = x?.ToString() ?? "-")
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.ApprovalCount)
                    .Subscribe(x => approvals.Text = x?.ToString() ?? "-")
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.CommentCount)
                    .Subscribe(x => commentCount.Text = x?.ToString() ?? "-")
                    .AddTo(disposable);

                this.WhenAnyObservable(x => x.ViewModel.MergeCommand)
                    .Subscribe(_ =>
                    {
                        var vc = new PullRequestApproveViewController(
                            ViewModel.Username, ViewModel.Repository, ViewModel.PullRequestId);
                        vc.DeleteSourceBranch = ViewModel.PullRequest.CloseSourceBranch;
                        vc.MergeCommand
                          .Do(x => ViewModel.PullRequest = x)
                          .Do(x => DismissViewController(true, null))
                          .Subscribe();
                        this.PresentModal(vc);
                    })
                    .AddTo(disposable);
            });
        }
    }
}

