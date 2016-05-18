using CodeBucket.Core.ViewModels.Users;
using CodeBucket.Core.Services;
using ReactiveUI;
using BitbucketSharp;
using System.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public class RepositoryWatchersViewModel : BaseUserCollectionViewModel
    {
        private readonly string _username, _repository;
        private readonly IApplicationService _applicationService;

        public RepositoryWatchersViewModel(
            string username, string repository, 
            IApplicationService applicationService = null)
        {
            _username = username;
            _repository = repository;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Watchers";
            EmptyMessage = "There are no watchers.";
        }

        protected override System.Threading.Tasks.Task Load(ReactiveList<UserItemViewModel> users)
        {
            return _applicationService.Client.ForAllItems(x => x.Repositories.GetWatchers(_username, _repository),
                                                          x => users.AddRange(x.Select(ToViewModel)));
        }
    }
}

