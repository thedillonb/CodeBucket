using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using BitbucketSharp.Models;
using System;
using ReactiveUI;
using System.Reactive;
using Splat;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Comments;
using Humanizer;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly Func<string, Task<CommentModel>> _addCommentFunc;

        private readonly ObservableAsPropertyHelper<string> _markdownDescription;
        public string MarkdownDescription => _markdownDescription.Value;

		private IssueModel _issueModel;
        public IssueModel Issue
        {
            get { return _issueModel; }
            private set { this.RaiseAndSetIfChanged(ref _issueModel, value); }
        }

        public IReactiveCommand<object> GoToAssigneeCommand { get; }

        public IReactiveCommand<object> GoToMilestoneCommand { get; }

        public IReactiveCommand<object> GoToEditCommand { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReadOnlyReactiveList<CommentItemViewModel> Comments { get; }

        public IReactiveCommand<object> GoToWebCommand { get; } = ReactiveCommand.Create();

        public IssueViewModel(
            string username, string repository, IssueModel issue,
            IApplicationService applicationService = null, IMarkdownService markdownService = null)
            : this(username, repository, issue.LocalId, applicationService, markdownService)
        {
            Issue = issue;
        }

        public IssueViewModel(
            string username, string repository, int issueId,
            IApplicationService applicationService = null, IMarkdownService markdownService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            markdownService = markdownService ?? Locator.Current.GetService<IMarkdownService>();

            _addCommentFunc = text => 
                applicationService.Client.Repositories.Issues.AddComment(username, repository, issueId, text);

            Title = "Issue #" + issueId;

            GoToWebCommand
                .OfType<string>()
                .Select(x => new WebBrowserViewModel(x))
                .Subscribe(NavigateTo);

            GoToAssigneeCommand = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.Issue.Responsible.Username).Select(x => x != null));
            
            GoToAssigneeCommand
                .Select(_ => new UserViewModel(Issue.Responsible.Username))
                .Subscribe(NavigateTo);

            GoToEditCommand = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.Issue).Select(x => x != null));

            GoToEditCommand
                .Select(_ => new IssueEditViewModel(username, repository, Issue))
                .Subscribe(NavigateTo);

            GoToMilestoneCommand = null;
            //              return new MvxCommand(() => {
            //                  if (Issue.Metadata != null && !string.IsNullOrEmpty(Issue.Metadata.Milestone))
            //                      GetService<IViewModelTxService>().Add(new IssueVersion { Name = Issue.Metadata.Milestone });
            //                  ShowViewModel<IssueMilestonesViewModel>(new IssueMilestonesViewModel.NavObject { Username = Username, Repository = Repository, Id = Id, SaveOnSelect = true });
            //              }); 

            GoToEditCommand.Subscribe(_ =>
            {

            });

            this.WhenAnyValue(x => x.Issue.Content)
                .Select(markdownService.ConvertMarkdown)
                .ToProperty(this, x => x.MarkdownDescription, out _markdownDescription);

            var comments = new ReactiveList<CommentModel>();
            Comments = comments.CreateDerivedCollection(x =>
            {
                return new CommentItemViewModel(x.AuthorInfo.Username,
                                                new Utils.Avatar(x.AuthorInfo.Avatar),
                                                x.UtcCreatedOn.Humanize(),
                                                markdownService.ConvertMarkdown(x.Content));

            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async t => {
                var issueTask = applicationService.Client.Repositories.Issues.GetIssue(username, repository, issueId);
                applicationService.Client.Repositories.Issues.GetComments(username, repository, issueId)
                                  .ToBackground(x => comments.Reset(x.Where(y => !string.IsNullOrEmpty(y.Content))));
                Issue = await issueTask;
            });
        }
    }
}

