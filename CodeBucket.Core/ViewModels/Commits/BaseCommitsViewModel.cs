using System.Windows.Input;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using CodeBucket.Client.Models.V2;
using CodeBucket.Core.Utils;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Commits
{
	public abstract class BaseCommitsViewModel : LoadableViewModel
	{
        private readonly CollectionViewModel<CommitModel> _commits = new CollectionViewModel<CommitModel>();

        public string Username { get; private set; }

        public string Repository { get; private set; }

		public ICommand GoToChangesetCommand
		{
            get 
            { 
                return new MvxCommand<CommitModel>(GoToCommit); 
            }
		}

        protected virtual void GoToCommit(CommitModel x)
        {
            var repo = new RepositoryIdentifier(x.Repository.FullName);
            ShowViewModel<CommitViewModel>(new CommitViewModel.NavObject { Username = repo.Owner, Repository = repo.Name, Node = x.Hash });
        }

        public CollectionViewModel<CommitModel> Commits
		{
			get { return _commits; }
		}

		public virtual void Init(NavObject navObject)
		{
			Username = navObject.Username;
			Repository = navObject.Repository;
		}

		protected override async Task Load()
		{
            var items = await GetRequest(null);
            SetMoreItems(items);
            Commits.Items.Reset(items.Values);
		}

        private void SetMoreItems(Collection<CommitModel> c)
        {
            if (c.Next == null)
            {
                Commits.MoreItems = null;
            }
            else
            {
                Commits.MoreItems = async () => {
                    var items = await GetRequest(c.Next);
                    Commits.Items.AddRange(items.Values);
                    SetMoreItems(items);
                };
            }
        }

        protected abstract Task<Collection<CommitModel>> GetRequest(string next);

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
	}
}

