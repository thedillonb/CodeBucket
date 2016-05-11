using System;
using System.Linq;
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
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestViewController : PrettyDialogViewController<PullRequestViewModel>
    {
        private readonly SplitViewElement _split1 = new SplitViewElement(AtlassianIcon.Calendar.ToImage(), AtlassianIcon.Devtoolsbranch.ToImage());
        private HtmlElement _descriptionElement = new HtmlElement("description");
        private HtmlElement _commentsElement = new HtmlElement("comments");

        public PullRequestViewController()
        {
            OnActivation(d =>
            {
                //d(_descriptionElement.UrlRequested.BindCommand(ViewModel.GoToUrlCommand));
                //d(_commentsElement.UrlRequested.BindCommand(ViewModel.GoToUrlCommand));
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var split = new SplitButtonElement();
            var comments = split.AddButton("Comments", "-");
            var participants = split.AddButton("Participants", "-");
            var approvals = split.AddButton("Approvals", "-");

            this.WhenAnyValue(x => x.ViewModel.ParticipantCount)
                     .Subscribe(x => participants.Text = x?.ToString() ?? "-");

            this.WhenAnyValue(x => x.ViewModel.ApprovalCount)
                     .Subscribe(x => approvals.Text = x?.ToString() ?? "-");

            this.WhenAnyValue(x => x.ViewModel.CommentCount)
                     .Subscribe(x => comments.Text = x?.ToString() ?? "-");

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

            ViewModel.WhenAnyValue(x => x.PullRequest).Subscribe(_ => Render(split));
            //ViewModel.BindCollection(x => x.Comments).Subscribe(_ => Render(split));
        }

        public void Render(SplitButtonElement split)
        {
            if (ViewModel.PullRequest == null)
                return;
  
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
            //approveElement.BindCaption(ViewModel.Bind(x => x.Approved, true).Select(x => x ? "Unapprove" : "Approve"));
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

            var commentsSection = new Section("Comments");
            root.Add(commentsSection);

            if (ViewModel.Comments.Count > 0)
            {
                var comments = ViewModel
                    .Comments
                    .Select(x => new Comment(x.Avatar.ToUrl(), x.Name, x.Content, x.CreatedOn))
                    .ToList();
                
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
   //         var composer = new Composer();
			//composer.NewComment(this, async (text) => {
   //             try
   //             {
			//		await composer.DoWorkAsync("Commenting...", () =>  ViewModel.AddComment(text));
			//		composer.CloseComposer();
   //             }
   //             catch (Exception ex)
   //             {
			//		AlertDialogService.ShowAlert("Unable to post comment!", ex.Message);
   //             }
   //             finally
   //             {
   //                 composer.EnableSendButton = true;
   //             }
   //         });
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

