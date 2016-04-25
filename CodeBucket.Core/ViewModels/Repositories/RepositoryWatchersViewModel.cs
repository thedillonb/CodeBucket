using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryWatchersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string User { get; private set; }

        public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public RepositoryWatchersViewModel(IApplicationService applicationService)
        {
            Title = "Watchers";
            EmptyMessage = "There are no watchers.";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Users.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetWatchers(User, Repository), 
                                                             x => Users.AddRange(x.Select(ToViewModel)));
            });
        }

        public void Init(NavObject navObject)
        {
            User = navObject.User;
            Repository = navObject.Repository;
        }

        public class NavObject
        {
            public string User { get; set; }
            public string Repository { get; set; }
        }
    }
}

