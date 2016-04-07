using CodeBucket.Core.ViewModels.Repositories;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesForkedViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public string Username { get; private set; }

        public string Repository { get; private set; }

        public IReactiveCommand LoadCommand { get; }

        public RepositoriesForkedViewModel(IApplicationService applicationService)
            : base(applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Repositories.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetForks(Username, Repository), Repositories.Items.AddRange);
            });
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}
