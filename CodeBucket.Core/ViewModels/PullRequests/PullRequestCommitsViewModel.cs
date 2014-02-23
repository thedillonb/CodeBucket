using CodeFramework.Core.ViewModels;
using System.Threading.Tasks;
using BitbucketSharp.Models.V2;
using Cirrious.MvvmCross.ViewModels;
using System.Windows.Input;
using CodeFramework.Core.Utils;
using BitbucketSharp.Models;
using CodeFramework.Core.Services;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestCommitsViewModel : LoadableViewModel
    {
        private PullRequestModel _pullRequest;

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public ulong PullRequestId { get; private set; }

        private readonly CollectionViewModel<CommitModel> _commits = new CollectionViewModel<CommitModel>();
        public CollectionViewModel<CommitModel> Commits
        {
            get { return _commits; }
        }

        public ICommand GoToChangesetCommand
        {
            get 
            { 
                return new MvxCommand<CommitModel>(x =>
                {
                    if (_pullRequest.Source.Repository == null)
                        GetService<IAlertDialogService>().Alert("Deleted Repository", "Unable to view commit. The source repository has been deleted.");
                    else
                    {
                        var repo = new RepositoryIdentifier(_pullRequest.Source.Repository.FullName);
                        ShowViewModel<ChangesetViewModel>(new ChangesetViewModel.NavObject { Username = repo.Owner, Repository = repo.Name, Node = x.Hash }); 
                    }
                });
            }
        }

		public void Init(NavObject navObject)
		{
            Username = navObject.Username;
            Repository = navObject.Repository;
			PullRequestId = navObject.PullRequestId;
		}

        protected override Task Load(bool forceCacheInvalidation)
        {
            var t1 = this.RequestModel(() => this.GetApplication().Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].Get(forceCacheInvalidation), x => _pullRequest = x);
            var t2 = Commits.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].PullRequests[PullRequestId].GetCommits(forceCacheInvalidation));
            return Task.WhenAll(t1, t2);
        }

		public new class NavObject
		{
            public string Username { get; set; }
            public string Repository { get; set; }
			public ulong PullRequestId { get; set; }
		}
    }
}

