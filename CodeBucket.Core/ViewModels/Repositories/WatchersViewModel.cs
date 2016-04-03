using CodeBucket.Core.ViewModels.User;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class WatchersViewModel : BaseUserCollectionViewModel, ILoadableViewModel
    {
        public string User { get; private set; }

        public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public WatchersViewModel(IApplicationService applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Users.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetWatchers(User, Repository), Users.Items.AddRange);
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

