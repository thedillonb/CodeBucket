using ReactiveUI;
using CodeBucket.Core.Services;
using BitbucketSharp;
using Splat;
using System.Reactive;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public RepositoriesStarredViewModel(
            string username = null,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            username = username ?? applicationService.Account.Username;

            Title = "Watched";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => 
            {
                RepositoryList.Clear();
                return applicationService.Client.ForAllItems(x => 
                    x.Repositories.GetRepositories(username), RepositoryList.AddRange);
            });
        }
    }
}

