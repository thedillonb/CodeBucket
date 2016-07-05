using CodeBucket.Core.Services;
using ReactiveUI;
using System.Threading.Tasks;
using CodeBucket.Client;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoriesForkedViewModel : RepositoriesViewModel
    {
        private readonly string _username, _repository;

        public RepositoriesForkedViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
            : base(applicationService)
        {
            _username = username;
            _repository = repository;
            Title = "Forked";
        }

        protected override Task Load(IApplicationService applicationService, IReactiveList<Repository> repositories)
        {
            return applicationService.Client.ForAllItems(x =>
                x.Repositories.GetForks(_username, _repository), repositories.AddRange);
        }
    }
}
