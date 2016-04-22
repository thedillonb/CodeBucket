using MvvmCross.Core.ViewModels;
using System;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Services;
using BitbucketSharp;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Humanizer;
using System.Linq;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel, ILoadableViewModel
    {
        public CollectionViewModel<PullRequestItemViewModel> PullRequests { get; } = new CollectionViewModel<PullRequestItemViewModel>();

        public string Username { get; private set; }

        public string Repository { get; private set; }

        public ReactiveUI.IReactiveCommand LoadCommand { get; }

		private PullRequestState _selectedFilter;
		public PullRequestState SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        public PullRequestsViewModel(IApplicationService applicationService)
		{
            LoadCommand = ReactiveUI.ReactiveCommand.CreateAsyncTask(_ =>
            {
                PullRequests.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.PullRequests.GetPullRequests(Username, Repository, SelectedFilter), 
                                                             x => PullRequests.Items.AddRange(x.Select(ToViewModel)));
            });

            this.Bind(x => x.SelectedFilter).BindCommand(LoadCommand);
        }

        private PullRequestItemViewModel ToViewModel(PullRequest pullRequest)
        {
            var avatar = new Avatar(pullRequest.Author?.Links?.Avatar?.Href);
            var vm = new PullRequestItemViewModel(pullRequest.Title, avatar, pullRequest.CreatedOn.Humanize());
            vm.GoToCommand
              .Select(x => new PullRequestViewModel.NavObject { Username = Username, Repository = Repository, Id = pullRequest.Id })
              .Subscribe(x => ShowViewModel<PullRequestViewModel>(x));
            return vm;
        }

		public void Init(NavObject navObject) 
        {
			Username = navObject.Username;
			Repository = navObject.Repository;
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}
