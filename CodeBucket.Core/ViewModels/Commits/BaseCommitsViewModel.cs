
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

namespace CodeBucket.Core.ViewModels.Commits
{
    public interface ICommitsViewModel
    {
        ReactiveList<CommitItemViewModel> Commits { get; }

        IReactiveCommand<Unit> LoadMoreCommand { get; }
    }

    public abstract class BaseCommitsViewModel : BaseViewModel, ILoadableViewModel, ICommitsViewModel
	{
        private string _nextUrl;

        public ReactiveList<CommitItemViewModel> Commits { get; } = new ReactiveList<CommitItemViewModel>();

        public string Username { get; protected set; }

        public string Repository { get; protected set; }

        protected IApplicationService ApplicationService { get; }

        public IReactiveCommand LoadCommand { get; }

        public IReactiveCommand<Unit> LoadMoreCommand { get; }

        private bool _hasMore;
        public bool HasMore
        {
            get { return _hasMore; }
            private set
            {
                if (_hasMore == value)
                    return;
                _hasMore = value;
                RaisePropertyChanged();
            }
        }

        protected BaseCommitsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                HasMore = false;
                var commits = await GetRequest();
                Commits.Reset(commits.Values.Select(ToViewModel));
                _nextUrl = commits.Next;
                HasMore = !string.IsNullOrEmpty(_nextUrl);
            });

            var hasMoreObs = this.Bind(x => x.HasMore, true);
            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(hasMoreObs, async _ =>
            {
                HasMore = false;
                var uri = new Uri(_nextUrl);
                var commits = await ApplicationService.Client.Get<Collection<Commit>>(uri);
                Commits.AddRange(commits.Values.Select(ToViewModel));
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
            vm.GoToCommand.Subscribe(_ =>
            {
                ShowViewModel<CommitViewModel>(new CommitViewModel.NavObject { Username = Username, Repository = Repository, Node = commit.Hash });
            });

            return vm;
        }

        protected abstract Task<Collection<Commit>> GetRequest();
	}
}

