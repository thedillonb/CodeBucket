using System;
using CodeBucket.Views;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.ViewControllers;
using MonoTouch.Dialog;
using CodeBucket.Utils;
using CodeBucket.Elements;
using System.Linq;
using CodeBucket.Core.ViewModels;
using Newtonsoft.Json;
using Cirrious.MvvmCross.ViewModels;

namespace CodeBucket.Views.Issues
{
	public class IssueView : ViewModelDrivenDialogViewController
    {
		private HeaderView _header;
		private WebElement _descriptionElement;
		private WebElement _commentsElement;


		public new IssueViewModel ViewModel
		{
			get { return (IssueViewModel) base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public IssueView()
		{
			Root.UnevenRows = true;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            _header = new HeaderView();

			var content = System.IO.File.ReadAllText("WebCell/body.html", System.Text.Encoding.UTF8);
            _descriptionElement = new WebElement(content, "description_webview", false);
			_descriptionElement.UrlRequested = ViewModel.GoToUrlCommand.Execute;

			var content2 = System.IO.File.ReadAllText("WebCell/comments.html", System.Text.Encoding.UTF8);
            _commentsElement = new WebElement(content2, "body_webview", true);
			_commentsElement.UrlRequested = ViewModel.GoToUrlCommand.Execute;

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Compose, (s, e) => ViewModel.GoToEditCommand.Execute(null));
			NavigationItem.RightBarButtonItem.Enabled = false;
			ViewModel.Bind(x => x.Issue, RenderIssue);
			ViewModel.BindCollection(x => x.Comments, (e) => RenderComments());
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			Title = "Issue #" + ViewModel.Id;
		}

		public void RenderComments()
		{
			var comments = ViewModel.Comments.Where(x => !string.IsNullOrEmpty(x.Content)).Select(x => new { 
				avatarUrl = x.AuthorInfo.Avatar, 
				login = x.AuthorInfo.Username, 
                created_at = x.UtcCreatedOn.ToDaysAgo(), 
				body = ViewModel.ConvertToMarkdown(x.Content)
			});

			var data = JsonConvert.SerializeObject(comments);
			InvokeOnMainThread(() => {
				_commentsElement.Value = data;
				if (_commentsElement.GetImmediateRootElement() == null)
					RenderIssue();
			});
		}

		public void RenderIssue()
		{
			if (ViewModel.Issue == null)
				return;

			NavigationItem.RightBarButtonItem.Enabled = true;

			var root = new RootElement(Title);
			_header.Title = ViewModel.Issue.Title;
			_header.Subtitle = "Updated " + ViewModel.Issue.UtcLastUpdated.ToDaysAgo();
			root.Add(new Section(_header));


			var secDetails = new Section();

			if (!string.IsNullOrEmpty(ViewModel.Issue.Content))
			{
				_descriptionElement.Value = ViewModel.MarkdownDescription;
				secDetails.Add(_descriptionElement);
			}

			var split1 = new SplitElement(new SplitElement.Row { Image1 = Images.Cog, Image2 = Images.Priority });
			split1.Value.Text1 = ViewModel.Issue.Status;
			split1.Value.Text2 = ViewModel.Issue.Priority;
			secDetails.Add(split1);


			var split2 = new SplitElement(new SplitElement.Row { Image1 = Images.Flag, Image2 = Images.ServerComponents });
			split2.Value.Text1 = ViewModel.Issue.Metadata.Kind;
			split2.Value.Text2 = ViewModel.Issue.Metadata.Component ?? "No Component";
			secDetails.Add(split2);


			var split3 = new SplitElement(new SplitElement.Row { Image1 = Images.SitemapColor, Image2 = Images.Milestone });
			split3.Value.Text1 = ViewModel.Issue.Metadata.Version ?? "No Version";
			split3.Value.Text2 = ViewModel.Issue.Metadata.Milestone ?? "No Milestone";
			secDetails.Add(split3);

			var assigneeElement = new StyledStringElement("Assigned", ViewModel.Issue.Responsible != null ? ViewModel.Issue.Responsible.Username : "Unassigned", UITableViewCellStyle.Value1) {
				Image = Images.Person,
				Accessory = UITableViewCellAccessory.DisclosureIndicator
			};
			assigneeElement.Tapped += () => ViewModel.GoToAssigneeCommand.Execute(null);
			secDetails.Add(assigneeElement);

			root.Add(secDetails);

            if (ViewModel.Comments.Any(x => !string.IsNullOrEmpty(x.Content)))
			{
				root.Add(new Section { _commentsElement });
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

