using System;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System.Linq;
using Humanizer;
using CodeBucket.Core.Utils;
using System.Collections.Generic;
using ReactiveUI;
using CodeBucket.ViewControllers.Comments;
using System.Reactive.Linq;

namespace CodeBucket.Views.Issues
{
    public class IssueViewController : PrettyDialogViewController<IssueViewModel>
    {
        private HtmlElement _descriptionElement = new HtmlElement("description");
        private HtmlElement _commentsElement = new HtmlElement("comments");

		public IssueViewController()
		{
            OnActivation(d =>
            {
                Observable.Merge(_descriptionElement.UrlRequested, _commentsElement.UrlRequested)
                    .Select(WebBrowserViewController.CreateWithNavbar)
                    .Subscribe(x => PresentViewController(x, true, null))
                    .AddTo(d);
            });
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            HeaderView.SetImage(null, Images.Avatar);

            var compose = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Compose);
   
            this.WhenAnyValue(x => x.ViewModel.Issue).Subscribe(x =>
            {
                if (x != null)
                {
                    var avatarUrl = x?.ReportedBy?.Avatar;
                    HeaderView.SubText = "Updated " + ViewModel.Issue.UtcLastUpdated.Humanize();
                    HeaderView.SetImage(new Avatar(avatarUrl).ToUrl(128), Images.Avatar);
                }
                else
                {
                    HeaderView.SetImage(null, Images.Avatar);
                    HeaderView.SubText = null;
                }
            });

            var split = new SplitButtonElement();
            var commentCount = split.AddButton("Comments", "-");
            var watchers = split.AddButton("Watchers", "-");

            ICollection<Section> root = new LinkedList<Section>();
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

            var split1 = new SplitViewElement(AtlassianIcon.Configure.ToImage(), AtlassianIcon.Error.ToImage());
            var split2 = new SplitViewElement(AtlassianIcon.Flag.ToImage(), AtlassianIcon.Spacedefault.ToImage());
            var split3 = new SplitViewElement(AtlassianIcon.Copyclipboard.ToImage(), AtlassianIcon.Calendar.ToImage());

            split1.Button1.Text = ViewModel.Issue.Status;
            split1.Button2.Text = ViewModel.Issue.Priority;
            secDetails.Add(split1);

            split2.Button1.Text = ViewModel.Issue.Metadata.Kind;
            split2.Button2.Text = ViewModel.Issue.Metadata.Component ?? "No Component";
            secDetails.Add(split2);

            split3.Button1.Text = ViewModel.Issue.Metadata.Version ?? "No Version";
            split3.Button2.Text = ViewModel.Issue.Metadata.Milestone ?? "No Milestone";
            secDetails.Add(split3);

            var assigneeElement = new ButtonElement("Assigned", string.Empty, UITableViewCellStyle.Value1)
            {
                Image = AtlassianIcon.User.ToImage(),
            };
            secDetails.Add(assigneeElement);

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

            OnActivation(d =>
            {
                compose
                    .GetClickedObservable()
                    .BindCommand(ViewModel.GoToEditCommand)
                    .AddTo(d);

                addComment
                    .Clicked
                    .Subscribe(_ => NewCommentViewController.Present(this, ViewModel.AddComment))
                    .AddTo(d);

                assigneeElement
                    .BindValue(this.WhenAnyValue(x => x.ViewModel.Assigned))
                    .AddTo(d);

                assigneeElement
                    .Clicked
                    .BindCommand(ViewModel.GoToAssigneeCommand)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Issue)
                    .Select(x => x != null)
                    .Subscribe(x => compose.Enabled = x)
                    .AddTo(d);

                this.WhenAnyObservable(x => x.ViewModel.Comments.CountChanged)
                    .StartWith(ViewModel.Comments.Count)
                    .Subscribe(x => commentCount.Text = x.ToString())
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Issue.FollowerCount)
                    .Subscribe(x => watchers.Text = x.ToString())
                    .AddTo(d);
            });
        }
    }
}

