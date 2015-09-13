using System;
using System.Linq;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.Utils;
using CodeBucket.Elements;
using Humanizer;
using CodeBucket.Core.Utils;

namespace CodeBucket.Views.PullRequests
{
    public class PullRequestView : PrettyDialogViewController
    {
        private readonly SplitElement _split1;

        public new PullRequestViewModel ViewModel
        {
            get { return (PullRequestViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public PullRequestView()
        {
            Root.UnevenRows = true;
            _split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Create, Image2 = Images.Merge }) { BackgroundColor = UIColor.White };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HeaderView.SetImage(null, Images.Avatar);

            ViewModel.Bind(x => x.PullRequest, Render);
            ViewModel.BindCollection(x => x.Comments, e => Render());
        }

		public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            Title = "Pull Request #" + ViewModel.PullRequestId;
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
			if (!string.IsNullOrEmpty(ViewModel.PullRequest.Description))
            {
				var desc = new MultilinedElement(ViewModel.PullRequest.Description.Trim()) 
                { 
                    BackgroundColor = UIColor.White,
                    CaptionColor = Theme.CurrentTheme.MainTitleColor, 
                    ValueColor = Theme.CurrentTheme.MainTextColor
                };
                desc.CaptionFont = desc.ValueFont;
                desc.CaptionColor = desc.ValueColor;
                secDetails.Add(desc);
            }

			var merged = ViewModel.Merged;

            _split1.Value.Text1 = ViewModel.PullRequest.CreatedOn.ToString("MM/dd/yy");
            _split1.Value.Text2 = merged ? "Merged" : "Not Merged";
            secDetails.Add(_split1);
            root.Add(secDetails);

            root.Add(new Section {
				new StyledStringElement("Commits", () => ViewModel.GoToCommitsCommand.Execute(null), Images.Commit),
            });

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
 
                root.Add(new Section { new StyledStringElement("Merge", mergeAction, Images.Fork) });
            }


            if (ViewModel.Comments.Items.Count > 0)
            {
                var commentsSec = new Section();
                foreach (var x in ViewModel.Comments.Where(x => !string.IsNullOrEmpty(x.Content.Raw) && x.Inline == null).OrderBy(x => (x.CreatedOn)))
                {
                    var name = x.User.DisplayName ?? x.User.Username ?? "Unknown";
                    var avatar = new Avatar(x.User.Links?.Avatar?.Href);
                    commentsSec.Add(new NameTimeStringElement(name, x.Content.Raw, x.CreatedOn, avatar.ToUrl(), Images.Avatar));
                }

                //Load more if there's more comments
//                if (model.MoreComments != null)
//                {
//                    var loadMore = new PaginateElement("Load More", "Loading...", 
//                                                       e => this.DoWorkNoHud(() => model.MoreComments(),
//                                          x => Utilities.ShowAlert("Unable to load more!", x.Message))) { AutoLoadOnVisible = false, Background = false };
//                    commentsSec.Add(loadMore);
//                }

                if (commentsSec.Elements.Count > 0)
                    root.Add(commentsSec);
            }


            var addComment = new StyledStringElement("Add Comment") { Image = Images.Pencil };
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

