using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Reactive;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesForkedViewModel : RepositoriesViewModel, ILoadableViewModel
    {
        public IReactiveCommand<Unit> LoadCommand { get; }

        public RepositoriesForkedViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Forked";

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => 
            {
                RepositoryList.Clear();
                return applicationService.Client.ForAllItems(x => 
                    x.Repositories.GetForks(username, repository), RepositoryList.AddRange);
            });
        }
    }
}
