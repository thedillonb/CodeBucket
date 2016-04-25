using System;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.Services;
using BitbucketSharp.Models.V2;
using BitbucketSharp;
using System.Reactive;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Users;
using System.Linq;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IApplicationService _applicationService;

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public int PullRequestId { get; private set; }

        public ReactiveUI.ReactiveList<PullRequestComment> Comments { get; } = new ReactiveUI.ReactiveList<PullRequestComment>();

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        private bool _merged;
        public bool Merged
        {
            get { return _merged; }
            set { this.RaiseAndSetIfChanged(ref _merged, value); }
        }

        private bool _approved;
        public bool Approved
        {
            get { return _approved; }
            private set { this.RaiseAndSetIfChanged(ref _approved, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            private set { this.RaiseAndSetIfChanged(ref _description, value); }
        }

        private int? _approvals;
        public int? ApprovalCount
        {
            get { return _approvals; }
            private set { this.RaiseAndSetIfChanged(ref _approvals, value); }
        }

        private int? _comments;
        public int? CommentCount
        {
            get { return _comments; }
            private set { this.RaiseAndSetIfChanged(ref _comments, value); }
        }

        private int? _participants;
        public int? ParticipantCount
        {
            get { return _participants; }
            private set { this.RaiseAndSetIfChanged(ref _participants, value); }
        }

        private PullRequest _pullRequest;
        public PullRequest PullRequest 
        { 
            get { return _pullRequest; }
            private set { this.RaiseAndSetIfChanged(ref _pullRequest, value); }
        }

        public ReactiveUI.IReactiveCommand<Unit> MergeCommand { get; }

        public ReactiveUI.IReactiveCommand<Unit> ToggleApproveButton { get; }

        public ReactiveUI.IReactiveCommand<object> GoToUserCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ICommand GoToCommitsCommand
		{
			get 
            { 
                return new MvxCommand(() => {
                    if (PullRequest?.Source?.Repository == null)
                    {
                        DisplayAlert("The author has deleted the source repository for this pull request.");
                    }
                    else
                    {
                        ShowViewModel<PullRequestCommitsViewModel>(new PullRequestCommitsViewModel.NavObject { Username = Username, Repository = Repository, PullRequestId = PullRequestId });
                    }
                }); 
            }
		}

        public PullRequestViewModel(IMarkdownService markdownService, IApplicationService applicationService)
        {
            _applicationService = applicationService;

            Comments.Changed.Subscribe(_ => CommentCount = Comments.Count);

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                PullRequest = await applicationService.Client.Repositories.PullRequests.GetPullRequest(Username, Repository, PullRequestId);
                Comments.Clear();
                await applicationService.Client.ForAllItems(x =>
                    x.Repositories.PullRequests.GetPullRequestComments(Username, Repository, PullRequestId), Comments.AddRange);
            });

            var canMerge = this.Bind(x => x.PullRequest, true).Select(x => string.Equals(x?.State, "open"));
            MergeCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(canMerge, async t =>
            {
                PullRequest = await applicationService.Client.Repositories.PullRequests.MergePullRequest(Username, Repository, PullRequestId);
            });

            GoToUserCommand
                .OfType<string>()
                .Select(x => new UserViewModel.NavObject { Username = x })
                .Subscribe(x => ShowViewModel<UserViewModel>(x));

            ToggleApproveButton = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                if (Approved)
                    await _applicationService.Client.Repositories.PullRequests.UnapprovePullRequest(Username, Repository, PullRequestId);
                else
                    await _applicationService.Client.Repositories.PullRequests.ApprovePullRequest(Username, Repository, PullRequestId);

                PullRequest = await applicationService.Client.Repositories.PullRequests.GetPullRequest(Username, Repository, PullRequestId);
            });

            ToggleApproveButton.ThrownExceptions
                .Subscribe(x => DisplayAlert("Unable to approve commit: " + x.Message).ToBackground());

            this.Bind(x => x.PullRequest, true).Subscribe(x =>
            {
                var username = applicationService.Account.Username;
                Approved = x?.Participants
                    .FirstOrDefault(y => string.Equals(y.User.Username, username, StringComparison.OrdinalIgnoreCase))
                    ?.Approved ?? false;

                ApprovalCount = x?.Participants.Select(y => y.Approved).Count() ?? 0;
                ParticipantCount = x?.Participants.Count ?? 0;
                Description = string.IsNullOrEmpty(x?.Description) ? null : markdownService.ConvertMarkdown(x.Description);
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
            PullRequestId = navObject.Id;
        }

        public async Task AddComment(string text)
        {
            var res = await _applicationService.Client.Repositories.PullRequests.CommentPullRequest(Username, Repository, PullRequestId, text);
            var comment = await _applicationService.Client.Repositories.PullRequests.GetPullRequestComment(Username, Repository, PullRequestId, res.CommentId);
            Comments.Add(comment);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public int Id { get; set; }
        }
    }
}
