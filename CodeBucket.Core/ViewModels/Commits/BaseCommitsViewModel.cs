
using System.Threading.Tasks;
using BitbucketSharp.Models.V2;
using CodeBucket.Core.Utils;
using System;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Services;

namespace CodeBucket.Core.ViewModels.Commits
{
    public interface ICommitsViewModel
    {
        CollectionViewModel<Commit> Commits { get; }
        IReactiveCommand<object> GoToCommitCommand { get; }
    }

    public abstract class BaseCommitsViewModel : BaseViewModel, ILoadableViewModel, ICommitsViewModel
	{
        public CollectionViewModel<Commit> Commits { get; } = new CollectionViewModel<Commit>();

        protected IApplicationService ApplicationService { get; }

        public IReactiveCommand<object> GoToCommitCommand { get; }

        public IReactiveCommand LoadCommand { get; }

        protected BaseCommitsViewModel(IApplicationService applicationService)
        {
            ApplicationService = applicationService;

            GoToCommitCommand = ReactiveCommand.Create();
            GoToCommitCommand.OfType<Commit>().Subscribe(x =>
            {
                var repo = new RepositoryIdentifier(x.Repository.FullName);
                ShowViewModel<CommitViewModel>(new CommitViewModel.NavObject { Username = repo.Owner, Repository = repo.Name, Node = x.Hash });
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var commits = await GetRequest();
                Commits.Items.Reset(commits.Values);
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
                    Commits.Items.AddRange(items.Values);
                    SetMoreItems(items);
                };
            }
        }

        protected abstract Task<Collection<Commit>> GetRequest();
	}
}

