using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryWatchersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public RepositoryWatchersViewModel(string username, string repository, 
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Watchers";
            EmptyMessage = "There are no watchers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Users.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetWatchers(username, repository), 
                                                             x => Users.AddRange(x.Select(ToViewModel)));
            });
        }
    }
}

