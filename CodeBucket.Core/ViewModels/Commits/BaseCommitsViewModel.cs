using System.Threading.Tasks;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using System;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using Humanizer;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Commits
{
    public abstract class BaseCommitsViewModel : BaseViewModel, ILoadableViewModel, 
        IPaginatableViewModel, IListViewModel<CommitItemViewModel>
	{
        private string _nextUrl, _username, _repository;

        public IReadOnlyReactiveList<CommitItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadCommand { get; }

        public IReactiveCommand<Unit> LoadMoreCommand { get; }

        private bool _hasMore;
        public bool HasMore
        {
            get { return _hasMore; }
            private set { this.RaiseAndSetIfChanged(ref _hasMore, value); }
        }

        protected BaseCommitsViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            _username = username;
            _repository = repository;
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Commits";

            var commitItems = new ReactiveList<Commit>();
            Items = commitItems.CreateDerivedCollection(ToViewModel);

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                HasMore = false;
                commitItems.Clear();
                var commits = await GetRequest();
                commitItems.AddRange(commits.Values);
                _nextUrl = commits.Next;
                HasMore = !string.IsNullOrEmpty(_nextUrl);
            });

            var hasMoreObs = this.WhenAnyValue(x => x.HasMore);
            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(hasMoreObs, async _ =>
            {
                HasMore = false;
                var uri = new Uri(_nextUrl);
                var commits = await applicationService.Client.Get<Collection<Commit>>(uri);
                commitItems.AddRange(commits.Values);
                _nextUrl = commits.Next;
                HasMore = !string.IsNullOrEmpty(_nextUrl);
            });
        }

        private CommitItemViewModel ToViewModel(Commit commit)
        {
            var msg = commit.Message ?? string.Empty;
            var firstLine = msg.IndexOf("\n", StringComparison.Ordinal);
            var desc = firstLine > 0 ? msg.Substring(0, firstLine) : msg;

            string username;
            if (commit?.Author?.User != null)
            {
                username = commit.Author.User.DisplayName ?? commit.Author.User.Username;
            }
            else
            {
                var bracketStart = commit.Author.Raw.IndexOf("<", StringComparison.Ordinal);
                username = commit.Author.Raw.Substring(0, bracketStart > 0 ? bracketStart : commit.Author.Raw.Length);
            }

            var avatar = new Avatar(commit.Author?.User?.Links?.Avatar?.Href);
            var vm = new CommitItemViewModel(username, desc, commit.Date.Humanize(), avatar);
            vm.GoToCommand
              .Select(_ => new CommitViewModel(_username, _repository, commit))
              .Subscribe(NavigateTo);
            return vm;
        }

        protected abstract Task<Collection<Commit>> GetRequest();
	}
}

