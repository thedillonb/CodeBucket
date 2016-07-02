using System.Threading.Tasks;
using System.Windows.Input;
using CodeBucket.Client.Models;
using MvvmCross.Core.ViewModels;
using System;

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
            this.Bind(x => x.SelectedFilter).BindCommand(LoadCommand);
		}

		public void Init(NavObject navObject) 
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
        }

        protected override async Task Load()
        {
			var state = "OPEN";
			if (SelectedFilter == 1)
				state = "MERGED";
			else if (SelectedFilter == 2)
				state = "DECLINED";

            PullRequests.Items.Clear();
            await this.GetApplication().Client.ForAllItems(x => x.PullRequests.Get(Username, Repository, state),
                                                     PullRequests.Items.AddRange);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}
