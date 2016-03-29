using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesSharedViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand LoadCommand { get; }

        public RepositoriesSharedViewModel(IApplicationService applicationService)
            : base(applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ =>
            {
                var username = applicationService.Account.Username;
                Repositories.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Users.GetRepositories(applicationService.Account.Username), repos => {
                    var shared = repos.Where(x => !string.Equals(x.Owner?.Username, username, System.StringComparison.OrdinalIgnoreCase));
                    Repositories.Items.AddRange(shared);
                });
            });
        }
    }
}

