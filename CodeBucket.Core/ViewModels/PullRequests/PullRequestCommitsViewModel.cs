using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Core.Services;
using Splat;
using CodeBucket.Client;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : BaseCommitsViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly string _username, _repository;
        private readonly int _pullRequestId;

        public PullRequestCommitsViewModel(
            string username, string repository, int pullRequestId,
            IApplicationService applicationService = null)
            : base(username, repository, applicationService)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _username = username;
            _repository = repository;
            _pullRequestId = pullRequestId;
        }

        protected override Task<Collection<Commit>> GetRequest()
        {
            return _applicationService.Client.PullRequests.GetCommits(_username, _repository, _pullRequestId);
        }
    }
}

