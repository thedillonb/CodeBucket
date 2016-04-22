using BitbucketSharp.Models.V2;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : BaseCommitsViewModel
    {
        public int PullRequestId { get; private set; }

        public PullRequestCommitsViewModel(IApplicationService applicationService)
            : base(applicationService)
        {
        }

        protected override System.Threading.Tasks.Task<Collection<Commit>> GetRequest()
        {
            return ApplicationService.Client.Commits.GetPullRequestCommits(Username, Repository, PullRequestId);
        }

		public void Init(NavObject navObject)
		{
            Username = navObject.Username;
            Repository = navObject.Repository;
			PullRequestId = navObject.PullRequestId;
		}

        public class NavObject
		{
            public string Username { get; set; }
            public string Repository { get; set; }
			public int PullRequestId { get; set; }
		}
    }
}

