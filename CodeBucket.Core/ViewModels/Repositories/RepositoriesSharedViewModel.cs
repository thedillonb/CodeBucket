using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesSharedViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public RepositoriesSharedViewModel(
            string username = null,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            username = username ?? applicationService.Account.Username;

            Title = "Shared";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => 
            {
                RepositoryList.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetRepositories(username), repos => 
                {
                    var shared = repos.Where(x => !string.Equals(x.Owner?.Username, username, System.StringComparison.OrdinalIgnoreCase));
                    RepositoryList.AddRange(shared);
                });
            });
        }
    }
}

