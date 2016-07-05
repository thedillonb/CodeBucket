using System;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using Humanizer;
using System.Linq;
using Splat;
using ReactiveUI;
using System.Reactive;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestsViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<PullRequestItemViewModel>
    {
        public IReadOnlyReactiveList<PullRequestItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

		private PullRequestState _selectedFilter;
		public PullRequestState SelectedFilter
		{
			get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
		}

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public PullRequestsViewModel(string username, string repository,
                                     IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Pull Requests";

            var pullRequests = new ReactiveList<PullRequest>();
            Items = pullRequests.CreateDerivedCollection(
                pullRequest =>
                {
                    var avatar = new Avatar(pullRequest.Author?.Links?.Avatar?.Href);
                    var vm = new PullRequestItemViewModel(pullRequest.Title, avatar, pullRequest.CreatedOn.Humanize());
                    vm.GoToCommand
                      .Select(_ => new PullRequestViewModel(username, repository, pullRequest))
                      .Subscribe(NavigateTo);
                    return vm;
                },
                x => x.Title.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                pullRequests.Clear();
                return applicationService.Client.ForAllItems(x => 
                    x.PullRequests.GetAll(username, repository, SelectedFilter), 
                    pullRequests.AddRange);
            });

            LoadCommand.IsExecuting.CombineLatest(pullRequests.IsEmptyChanged, (x, y) => !x && y)
                       .ToProperty(this, x => x.IsEmpty, out _isEmpty);

            this.WhenAnyValue(x => x.SelectedFilter)
                .Skip(1)
                .InvokeCommand(LoadCommand);
        }
    }
}
