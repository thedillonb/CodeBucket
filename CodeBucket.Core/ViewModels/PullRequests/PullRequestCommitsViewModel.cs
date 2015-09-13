using BitbucketSharp.Models.V2;
using CodeBucket.Core.ViewModels.Commits;
using BitbucketSharp.Models;
using CodeBucket.Core.Utils;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : BaseCommitsViewModel
    {
        private PullRequestModel _pullRequest;

        public ulong PullRequestId { get; private set; }

		public void Init(NavObject navObject)
		{
			PullRequestId = navObject.PullRequestId;
            base.Init(navObject);
		}

        protected override void GoToCommit(CommitModel x)
        {
            if (_pullRequest?.Source?.Repository?.FullName == null)
            {
                DisplayAlert("Unable to locate the source repository for this pull request. It may have been deleted!");
            }
            else
            {
                var repo = new RepositoryIdentifier(_pullRequest.Source.Repository.FullName);
                ShowViewModel<CommitViewModel>(new CommitViewModel.NavObject { Username = repo.Owner, Repository = repo.Name, Node = x.Hash });
            }
        }

        protected override Collection<CommitModel> GetRequest(string next)
        {
            return next == null ? 
                this.GetApplication().Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].GetCommits() : 
                this.GetApplication().Client.Request2<Collection<CommitModel>>(next);
        }

        protected override async Task Load(bool forceCacheInvalidation)
        {
            _pullRequest = await Task.Run(() => this.GetApplication().Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].Get());
            await base.Load(forceCacheInvalidation);
        }

        public new class NavObject : CommitsViewModel.NavObject
		{
			public ulong PullRequestId { get; set; }
		}
    }
}

