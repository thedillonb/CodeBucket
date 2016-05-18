using ReactiveUI;
using CodeBucket.Core.Services;
using BitbucketSharp;
using BitbucketSharp.Models.V2;
using System.Threading.Tasks;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesStarredViewModel : RepositoriesViewModel
    {
        private readonly string _username;

        public RepositoriesStarredViewModel(
            string username = null,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _username = username ?? applicationService.Account.Username;
            Title = "Watched";
        }

        protected override Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories)
        {
            return applicationService.Client.ForAllItems(x =>
                x.Repositories.GetRepositories(_username), repositories.AddRange);
        }
    }
}

