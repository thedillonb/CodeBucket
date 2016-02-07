using System;
using System.Linq;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.Utils;
using CodeBucket.Elements;
using Humanizer;
using CodeBucket.Core.Utils;
using CodeBucket.WebCell;
using CodeBucket.Core.Services;
using CodeBucket.DialogElements;

namespace CodeBucket.Views.PullRequests
{
    public class PullRequestView : PrettyDialogViewController
    {
        private readonly SplitViewElement _split1 = new SplitViewElement(AtlassianIcon.Calendar.ToImage(), AtlassianIcon.Devtoolsbranch.ToImage());
        private WebElement _descriptionElement;
        private WebElement _commentsElement;

        public new PullRequestViewModel ViewModel
        {
            get { return (PullRequestViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public PullRequestView()
        {
            Root.UnevenRows = true;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = "Pull Request #" + ViewModel.PullRequestId;

            HeaderView.SetImage(null, Images.Avatar);

            ViewModel.Bind(x => x.PullRequest, Render);
            ViewModel.BindCollection(x => x.Comments, e => Render());
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

            var root = new RootElement(Title);
            root.Add(new Section { split });

            var secDetails = new Section();
            if (!string.IsNullOrWhiteSpace(ViewModel.Description))
            {
                if (_descriptionElement == null)
                {
                    _descriptionElement = new WebElement("description");
                    _descriptionElement.UrlRequested = ViewModel.GoToUrlCommand.Execute;
                }

                _descriptionElement.LoadContent(new MarkdownRazorView { Model = ViewModel.Description }.GenerateString());
                secDetails.Add(_descriptionElement);
            }

			var merged = ViewModel.Merged;

            _split1.Button1.Text = ViewModel.PullRequest.CreatedOn.ToString("MM/dd/yy");
            _split1.Button2.Text = merged ? "Merged" : "Not Merged";
            secDetails.Add(_split1);
            secDetails.Add(new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null), AtlassianIcon.Devtoolscommit.ToImage()));
            root.Add(secDetails);

            if (!merged)
            {
                Action mergeAction = async () =>
                {
                    try
                    {
						await this.DoWorkAsync("Merging...", ViewModel.Merge);
                    }
                    catch (Exception e)
                    {
                        MonoTouch.Utilities.ShowAlert("Unable to Merge", e.Message);
                    }
                };
 
                root.Add(new Section { new StyledStringElement("Merge", mergeAction, AtlassianIcon.Approve.ToImage()) });
            }

            var comments = ViewModel.Comments
                .Where(x => !string.IsNullOrEmpty(x.Content.Raw) && x.Inline == null)
                .OrderBy(x => (x.CreatedOn))
                .Select(x =>
                {
                    var name = x.User.DisplayName ?? x.User.Username ?? "Unknown";
                    var avatar = new Avatar(x.User.Links?.Avatar?.Href);
                    return new CommentViewModel(name, x.Content.Html, x.CreatedOn, avatar.ToUrl());
                }).ToList();

            if (comments.Count > 0)
            {
                if (_commentsElement == null)
                {
                    _commentsElement = new WebElement("comments");
                    _commentsElement.UrlRequested = ViewModel.GoToUrlCommand.Execute;
                }

                _commentsElement.LoadContent(new CommentsRazorView { Model = comments.ToList() }.GenerateString());
                root.Add(new Section { _commentsElement });
            }


            var addComment = new StyledStringElement("Add Comment") { Image = AtlassianIcon.Addcomment.ToImage() };
            addComment.Tapped += AddCommentTapped;
            root.Add(new Section { addComment });
            Root = root;
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
					MonoTouch.Utilities.ShowAlert("Unable to post comment!", ex.Message);
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

