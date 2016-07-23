using System;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using System.Linq;
using Splat;
using ReactiveUI;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel
    {
        private PullRequestListViewModel _listViewModel;
        public PullRequestListViewModel PullRequests
        {
            get { return _listViewModel; }
            private set { this.RaiseAndSetIfChanged(ref _listViewModel, value); }
        }

        private PullRequestState _selectedFilter;
        public PullRequestState SelectedFilter
        {
            get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
        }

        public PullRequestsViewModel(string username, string repository,
                                     IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Pull Requests";

            this.WhenAnyValue(x => x.SelectedFilter)
                .Select(x => new PullRequestListViewModel(username, repository, x))
                .Do(x => x.LoadMoreCommand.ExecuteIfCan())
                .Subscribe(x => PullRequests = x);

            this.WhenAnyValue(x => x.PullRequests)
                .Select(x => x.Selected)
                .Switch()
                .Select(x => new PullRequestViewModel(username, repository, x))
                .Subscribe(NavigateTo);
        }
    }
}
