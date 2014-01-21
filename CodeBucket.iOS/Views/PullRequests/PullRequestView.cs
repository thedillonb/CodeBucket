using System;
using System.Linq;
using CodeFramework.Elements;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Views;
using CodeBucket.Core.ViewModels.PullRequests;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.iOS.Utils;

namespace CodeBucket.iOS.Views.PullRequests
{
	public class PullRequestView : ViewModelDrivenDialogViewController
    {
        private readonly HeaderView _header;
        private readonly SplitElement _split1;

        public new PullRequestViewModel ViewModel
        {
            get { return (PullRequestViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public PullRequestView()
        {
            Root.UnevenRows = true;
            _header = new HeaderView() { ShadowImage = false };
            _split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Create, Image2 = Images.Merge }) { BackgroundColor = UIColor.White };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel.Bind(x => x.PullRequest, Render);
            ViewModel.BindCollection(x => x.Comments, e => Render());
        }

		public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            Title = "Pull Request #".t() + ViewModel.PullRequestId;
        }

        public void Render()
        {
            if (ViewModel.PullRequest == null)
                return;

            var root = new RootElement(Title);
            _header.Title = ViewModel.PullRequest.Title;
			_header.Subtitle = "Updated " + (ViewModel.PullRequest.UpdatedOn).ToDaysAgo();
            _header.SetNeedsDisplay();
            root.Add(new Section(_header));

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

//            if (!merged)
//            {
//                MonoTouch.Foundation.NSAction mergeAction = async () =>
//                {
//                    try
//                    {
//						await this.DoWorkAsync("Merging...", ViewModel.Merge);
//                    }
//                    catch (Exception e)
//                    {
//                        MonoTouch.Utilities.ShowAlert("Unable to Merge", e.Message);
//                    }
//                };
// 
//                root.Add(new Section { new StyledStringElement("Merge".t(), mergeAction, Images.Fork) });
//            }


            if (ViewModel.Comments.Items.Count > 0)
            {
                var commentsSec = new Section();
                foreach (var x in ViewModel.Comments.Where(x => !string.IsNullOrEmpty(x.Content.Raw) && x.Inline == null).OrderBy(x => (x.CreatedOn)))
                {
                    commentsSec.Add(new CommentElement
                    {
                        Name = x.User.Username,
                        Time = x.CreatedOn.ToDaysAgo(),
                        String = x.Content.Raw,
                        Image = Theme.CurrentTheme.AnonymousUserImage,
                        ImageUri = new Uri(x.User.Links.Avatar.Href),
                        BackgroundColor = UIColor.White,
                    });
                }

                //Load more if there's more comments
//                if (model.MoreComments != null)
//                {
//                    var loadMore = new PaginateElement("Load More".t(), "Loading...".t(), 
//                                                       e => this.DoWorkNoHud(() => model.MoreComments(),
//                                          x => Utilities.ShowAlert("Unable to load more!".t(), x.Message))) { AutoLoadOnVisible = false, Background = false };
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
					await composer.DoWorkAsync("Commenting...".t(), () =>  ViewModel.AddComment(text));
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
                var u = new UIView(new System.Drawing.RectangleF(0, 0, 320f, 27)) { BackgroundColor = UIColor.White };
                return u;
            }
        }
    }
}

