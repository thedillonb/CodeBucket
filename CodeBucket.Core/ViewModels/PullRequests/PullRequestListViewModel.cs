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
using System.Reactive.Subjects;

namespace CodeBucket.Core.ViewModels.PullRequests
{
    public class PullRequestListViewModel : ReactiveObject, IPaginatableViewModel, IListViewModel<PullRequestItemViewModel>
    {
        private readonly ISubject<PullRequest> _selectSubject = new Subject<PullRequest>();

        public IObservable<PullRequest> Selected => _selectSubject.AsObservable();

        public IReadOnlyReactiveList<PullRequestItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadMoreCommand { get; }

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

        private bool _hasMoreItems;
        public bool HasMoreItems
        {
            get { return _hasMoreItems; }
            set { this.RaiseAndSetIfChanged(ref _hasMoreItems, value); }
        }

        public PullRequestListViewModel(
            string username, string repository, PullRequestState state,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var pullRequests = new ReactiveList<PullRequest>(resetChangeThreshold: 10);
            Items = pullRequests.CreateDerivedCollection(
                pullRequest =>
                {
                    var avatar = new Avatar(pullRequest.Author?.Links?.Avatar?.Href);
                    var vm = new PullRequestItemViewModel(pullRequest.Title, avatar, pullRequest.CreatedOn.Humanize());
                    vm.GoToCommand
                      .Select(_ => pullRequest)
                      .Subscribe(_selectSubject);
                    return vm;
                },
                x => x.Title.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            string nextPage = null;
            HasMoreItems = true;

            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.HasMoreItems),
                async _ =>
                {
                    if (nextPage == null)
                    {
                        var x = await applicationService.Client.PullRequests.GetAll(username, repository, state);
                        pullRequests.Reset(x.Values);
                        nextPage = x.Next;
                    }
                    else
                    {
                        var x = await applicationService.Client.Get<Collection<PullRequest>>(nextPage);
                        pullRequests.AddRange(x.Values);
                        nextPage = x.Next;
                    }

                    HasMoreItems = nextPage != null;
                });

            LoadMoreCommand.IsExecuting.CombineLatest(pullRequests.IsEmptyChanged, (x, y) => !x && y)
                .ToProperty(this, x => x.IsEmpty, out _isEmpty);
        }
    }
}
