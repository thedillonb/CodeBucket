using CodeBucket.Client.Models.V2;
using CodeBucket.Core.ViewModels.Commits;
using CodeBucket.Client.Models;
using CodeBucket.Core.Utils;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : BaseCommitsViewModel
    {
        private PullRequestModel _pullRequest;

        public int Id { get; private set; }

		public void Init(NavObject navObject)
		{
			Id = navObject.PullRequestId;
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

        protected override async Task<Collection<CommitModel>> GetRequest(string next)
        {
            return await (next == null ? 
                this.GetApplication().Client.PullRequests.GetCommits(Username, Repository, Id) :
                this.GetApplication().Client.Get<Collection<CommitModel>>(next));
        }

        protected override async Task Load()
        {
            _pullRequest = await this.GetApplication().Client.PullRequests.Get(Username, Repository, Id);
            await base.Load();
        }

        public new class NavObject : CommitsViewModel.NavObject
		{
			public int PullRequestId { get; set; }
		}
    }
}

