using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class UserRepositoriesViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public UserRepositoriesViewModel(string username, IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                RepositoryList.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetRepositories(username), RepositoryList.AddRange);
            });
        }
    }
}
