using ReactiveUI;
using CodeBucket.Core.Services;
using BitbucketSharp;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand LoadCommand { get; }

        public RepositoriesStarredViewModel(IApplicationService applicationService)
            : base(applicationService)
        {
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => {
                Repositories.Items.Clear();
                return applicationService.Client.ForAllItems(x => x.Repositories.GetRepositories(applicationService.Account.Username), Repositories.Items.AddRange);
            });
        }
    }
}

