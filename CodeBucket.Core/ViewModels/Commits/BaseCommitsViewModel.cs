
using System.Threading.Tasks;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using System;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;
using Humanizer;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Commits
{
    public interface ICommitsViewModel
    {
        CollectionViewModel<CommitItemViewModel> Commits { get; }
    }

    public abstract class BaseCommitsViewModel : BaseViewModel, ILoadableViewModel, ICommitsViewModel
	{
        public CollectionViewModel<CommitItemViewModel> Commits { get; } = new CollectionViewModel<CommitItemViewModel>();

        public string Username { get; protected set; }

        public string Repository { get; protected set; }

        protected IApplicationService ApplicationService { get; }

        public IReactiveCommand LoadCommand { get; }

        protected BaseCommitsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var commits = await GetRequest();
                Commits.Items.Reset(commits.Values.Select(ToViewModel));
                SetMoreItems(commits);
            });
        }

        private void SetMoreItems(Collection<Commit> c)
        {
            if (c.Next == null)
            {
                Commits.MoreItems = null;
            }
            else
            {
                var uri = new Uri(c.Next);
                Commits.MoreItems = async () => {
                    var items = await ApplicationService.Client.Get<Collection<Commit>>(uri);
                    Commits.Items.AddRange(items.Values.Select(ToViewModel));
                    SetMoreItems(items);
                };
            }
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

