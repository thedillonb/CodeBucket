using System.Threading.Tasks;
using System.Windows.Input;
using CodeFramework.Core.ViewModels;
using BitbucketSharp.Models;
using Cirrious.MvvmCross.ViewModels;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : LoadableViewModel
    {
		private readonly CollectionViewModel<PullRequestModel> _pullrequests = new CollectionViewModel<PullRequestModel>();
		public CollectionViewModel<PullRequestModel> PullRequests
        {
            get { return _pullrequests; }
        }

        public string Username { get; private set; }

        public string Repository { get; private set; }

		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
			set 
			{
				_selectedFilter = value;
				RaisePropertyChanged(() => SelectedFilter);
			}
		}

        public ICommand GoToPullRequestCommand
        {
			get { return new MvxCommand<PullRequestModel>(x => ShowViewModel<PullRequestViewModel>(new PullRequestViewModel.NavObject { Username = Username, Repository = Repository, Id = x.Id })); }
        }

		public PullRequestsViewModel()
		{
			this.Bind(x => x.SelectedFilter, () => LoadCommand.Execute(null));
		}

		public void Init(NavObject navObject) 
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
        }

        protected override Task Load(bool forceCacheInvalidation)
        {
			var state = "OPEN";
			if (SelectedFilter == 1)
				state = "MERGED";
			else if (SelectedFilter == 2)
				state = "DECLINED";
			return PullRequests.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].PullRequests.GetAll(state, forceCacheInvalidation));
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}
