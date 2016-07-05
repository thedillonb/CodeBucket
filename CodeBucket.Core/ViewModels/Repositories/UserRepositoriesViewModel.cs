using CodeBucket.Core.Services;
using ReactiveUI;
using System.Threading.Tasks;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class UserRepositoriesViewModel : RepositoriesViewModel
    {
        private readonly string _username;

        public UserRepositoriesViewModel(string username, IApplicationService applicationService = null)
            : base(applicationService)
        {
            _username = username;
        }

        protected override Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories)
        {
            return applicationService.Client.ForAllItems(x => x.Repositories.GetAll(_username), repositories.AddRange);
        }
    }
}
