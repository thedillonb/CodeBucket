using MvvmCross.Core.ViewModels;
using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using BitbucketSharp;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel, ILoadableViewModel
    {
        public CollectionViewModel<PullRequest> PullRequests { get; } = new CollectionViewModel<PullRequest>();

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

        public ReactiveUI.IReactiveCommand<object> GoToPullRequestCommand { get; }

		private int _selectedFilter;
		public int SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public PullRequestsViewModel(IApplicationService applicationService)
		{
            this.Bind(x => x.SelectedFilter).BindCommand(LoadCommand);

            GoToPullRequestCommand = ReactiveUI.ReactiveCommand.Create();
            GoToPullRequestCommand
                .OfType<PullRequest>()
                .Select(x => new PullRequestViewModel.NavObject { Username = Username, Repository = Repository, Id = x.Id })
                .Subscribe(x => ShowViewModel<PullRequestViewModel>(x));

            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(_ =>
            {
                PullRequests.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetPullRequests(Username, Repository, GetState()), PullRequests.Items.AddRange);
            });
		}

		public void Init(NavObject navObject) 
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
        }

        private PullRequestState GetState()
        {
            switch (SelectedFilter)
            {
                case 1: return PullRequestState.Merged;
                case 2: return PullRequestState.Declined;
                default: return PullRequestState.Open;
            }
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}
