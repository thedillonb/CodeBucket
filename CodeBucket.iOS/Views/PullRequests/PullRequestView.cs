using System;
using System.Linq;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.DialogElements;
using Humanizer;
using CodeBucket.Core.Utils;
using System.Collections.Generic;
using CodeBucket.Utilities;
using CodeBucket.Services;
using ReactiveUI;
using System.Reactive.Linq;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.ViewModels.Users;

namespace CodeBucket.Views.PullRequests
{
    public class PullRequestView : PrettyDialogViewController
    {
        private readonly SplitViewElement _split1 = new SplitViewElement(AtlassianIcon.Calendar.ToImage(), AtlassianIcon.Devtoolsbranch.ToImage());
        private HtmlElement _descriptionElement = new HtmlElement("description");
        private HtmlElement _commentsElement = new HtmlElement("comments");

        public new PullRequestViewModel ViewModel
        {
            get { return (PullRequestViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public PullRequestView()
        {
            OnActivation(d =>
            {
                d(_descriptionElement.UrlRequested.BindCommand(ViewModel.GoToUrlCommand));
                d(_commentsElement.UrlRequested.BindCommand(ViewModel.GoToUrlCommand));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Pull Request #" + ViewModel.PullRequestId;

            HeaderView.SetImage(null, Images.Avatar);

            ViewModel.Bind(x => x.PullRequest).Subscribe(_ => Render());
            ViewModel.BindCollection(x => x.Comments).Subscribe(_ => Render());
        }

        public void Render()
        {
            if (ViewModel.PullRequest == null)
                return;

            var avatarUrl = ViewModel.PullRequest.Author?.Links?.Avatar?.Href;

            HeaderView.Text = ViewModel.PullRequest.Title;
            HeaderView.SubText = "Updated " + ViewModel.PullRequest.UpdatedOn.Humanize();
            HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
            RefreshHeaderView();

            var split = new SplitButtonElement();
            split.AddButton("Comments", ViewModel.Comments.Items.Count.ToString());
            split.AddButton("Participants", ViewModel.PullRequest.Participants.Count.ToString());

            ICollection<Section> root = new LinkedList<Section>();
            root.Add(new Section { split });

            var secDetails = new Section();
            if (!string.IsNullOrWhiteSpace(ViewModel.Description))
            {
                var model = new DescriptionModel(ViewModel.Description, (int)UIFont.PreferredSubheadline.PointSize, true);
                var content = new MarkdownView { Model = model }.GenerateString();
                _descriptionElement.SetValue(content);
                secDetails.Add(_descriptionElement);
            }

            var commitsElement = new StringElement("Commits", AtlassianIcon.Devtoolscommit.ToImage());
            commitsElement.Clicked.BindCommand(ViewModel.GoToCommitsCommand);

			var merged = ViewModel.Merged;

            _split1.Button1.Text = ViewModel.PullRequest.CreatedOn.ToString("MM/dd/yy");
            _split1.Button2.Text = merged ? "Merged" : "Not Merged";
            secDetails.Add(_split1);
            secDetails.Add(commitsElement);
            root.Add(secDetails);

            if (!merged)
            {
                var mergeElement = new StringElement("Merge", AtlassianIcon.Approve.ToImage());
                mergeElement.Clicked.InvokeCommand(ViewModel.MergeCommand);
                root.Add(new Section { mergeElement });
            }

            var approvalSection = new Section("Approvals");
            var approveElement = new LoaderButtonElement("Approve", AtlassianIcon.Approve.ToImage());
            approveElement.Accessory = UITableViewCellAccessory.None;
            approveElement.BindLoader(ViewModel.ToggleApproveButton);
            approveElement.BindCaption(ViewModel.Bind(x => x.Approved, true).Select(x => x ? "Decline" : "Approve"));
            root.Add(approvalSection);

            var participantElements = (ViewModel.PullRequest.Participants ?? Enumerable.Empty<Participant>())
                 .Where(y => y.Approved)
                 .Select(l =>
                 {
                     var avatar = new Avatar(l.User?.Links?.Avatar?.Href);
                     var vm = new UserItemViewModel(l.User.Username, l.User.DisplayName, avatar);
                     vm.GoToCommand.Select(_ => l.User.Username).BindCommand(ViewModel.GoToUserCommand);
                     return new UserElement(vm);
                 })
                .OfType<Element>();

            approvalSection.Reset(participantElements.Concat(new[] { approveElement }));

            var comments = ViewModel.Comments
                .Where(x => !string.IsNullOrEmpty(x.Content.Raw) && x.Inline == null)
                .OrderBy(x => (x.CreatedOn))
                .Select(x =>
                {
                    var name = x.User.DisplayName ?? x.User.Username ?? "Unknown";
                    var avatar = new Avatar(x.User.Links?.Avatar?.Href);
                    return new Comment(avatar.ToUrl(), name, x.Content.Html, x.CreatedOn);
                }).ToList();

            var commentsSection = new Section("Comments");
            root.Add(commentsSection);

            if (comments.Count > 0)
            {
                var commentModel = new CommentModel(comments, (int)UIFont.PreferredSubheadline.PointSize);
                var content = new CommentsView { Model = commentModel }.GenerateString();
                _commentsElement.SetValue(content);
                commentsSection.Add(_commentsElement);
            }

            var addComment = new StringElement("Add Comment") { Image = AtlassianIcon.Addcomment.ToImage() };
            addComment.Clicked.Subscribe(_ => AddCommentTapped());
            commentsSection.Add(addComment);

            Root.Reset(root);
        }

        void AddCommentTapped()
        {
            var composer = new Composer();
			composer.NewComment(this, async (text) => {
                try
                {
					await composer.DoWorkAsync("Commenting...", () =>  ViewModel.AddComment(text));
					composer.CloseComposer();
                }
                catch (Exception ex)
                {
					AlertDialogService.ShowAlert("Unable to post comment!", ex.Message);
                }
                finally
                {
                    composer.EnableSendButton = true;
                }
            });
        }

        public override UIView InputAccessoryView
        {
            get
            {
                var u = new UIView(new CoreGraphics.CGRect(0, 0, 320f, 27)) { BackgroundColor = UIColor.White };
                return u;
            }
        }
    }
}

