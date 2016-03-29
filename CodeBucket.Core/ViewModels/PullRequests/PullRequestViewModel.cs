using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using BitbucketSharp.Models.V2;
using BitbucketSharp;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestViewModel : BaseViewModel, ILoadableViewModel
    {
        private readonly IMarkdownService _markdownService;
        private readonly IApplicationService _applicationService;

        public string User { get; private set; }

        public string Repo { get; private set; }

        public int PullRequestId { get; private set; }

        public CollectionViewModel<PullRequestComment> Comments { get; } = new CollectionViewModel<PullRequestComment>();

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        private bool _merged;
        public bool Merged
        {
            get { return _merged; }
            set { this.RaiseAndSetIfChanged(ref _merged, value); }
        }

        public string Description { get; private set; }

        private PullRequest _pullRequest;
        public PullRequest PullRequest 
        { 
            get { return _pullRequest; }
            private set { this.RaiseAndSetIfChanged(ref _pullRequest, value); }
        }

        public ReactiveUI.IReactiveCommand<Unit> MergeCommand { get; }

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
                        ShowViewModel<PullRequestCommitsViewModel>(new PullRequestCommitsViewModel.NavObject { Username = User, Repository = Repo, PullRequestId = PullRequestId });
                    }
                }); 
            }
		}

        public PullRequestViewModel(IMarkdownService markdownService, IApplicationService applicationService)
        {
            _markdownService = markdownService;
            _applicationService = applicationService;

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(async _ =>
            {
                PullRequest = await applicationService.Client.Repositories.GetPullRequest(User, Repo, PullRequestId);
                await applicationService.Client.ForAllItems(x => x.Repositories.GetPullRequestComments(User, Repo, PullRequestId), Comments.Items.AddRange);
            });

            var canMerge = this.Bind(x => x.PullRequest, true).Select(x => string.Equals(x?.State, "open"));
            MergeCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(canMerge, async t =>
            {
                PullRequest = await applicationService.Client.Repositories.MergePullRequest(User, Repo, PullRequestId);
            });
        }

        public void Init(NavObject navObject)
        {
            User = navObject.Username;
            Repo = navObject.Repository;
            PullRequestId = navObject.Id;
        }

        public async Task AddComment(string text)
        {
            var res = await _applicationService.Client.Repositories.CommentPullRequest(User, Repo, PullRequestId, text);
            var comment = await _applicationService.Client.Repositories.GetPullRequestComment(User, Repo, PullRequestId, res.CommentId);
            Comments.Items.Add(comment);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
            public int Id { get; set; }
        }
    }
}
