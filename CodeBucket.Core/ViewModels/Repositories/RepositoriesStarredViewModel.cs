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
            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => 
            {
                RepositoryList.Clear();
                return applicationService.Client.ForAllItems(x => 
                    x.Repositories.GetRepositories(applicationService.Account.Username), RepositoryList.AddRange);
            });
        }
    }
}

