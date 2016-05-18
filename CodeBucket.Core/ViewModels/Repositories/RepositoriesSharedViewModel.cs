using System.Linq;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Threading.Tasks;
using BitbucketSharp.Models.V2;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesSharedViewModel : RepositoriesViewModel
    {
        private readonly string _username;

        public RepositoriesSharedViewModel(
            string username = null,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _username = username ?? applicationService.Account.Username;
            Title = "Shared";
        }

        protected override Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories)
        {
            return applicationService.Client.ForAllItems(x => x.Users.GetRepositories(_username), repos =>
            {
                var shared = repos.Where(x => !string.Equals(x.Owner?.Username, _username, System.StringComparison.OrdinalIgnoreCase));
                repositories.AddRange(shared);
            });
        }
    }
}

