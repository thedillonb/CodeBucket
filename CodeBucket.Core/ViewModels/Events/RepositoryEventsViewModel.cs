using CodeBucket.Core.Services;
using System.Threading.Tasks;
using Splat;

namespace CodeBucket.Core.ViewModels.Events
{
    public class RepositoryEventsViewModel : BaseEventsViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly string _username, _repository;

        public RepositoryEventsViewModel(string username, string repository, 
                                         IApplicationService applicationService = null)
        {
            _username = username;
            _repository = repository;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
        }

        protected override Task<BitbucketSharp.Models.EventsModel> GetEvents(int start, int limit)
        {
            return _applicationService.Client.Repositories.GetEvents(_username, _repository, start, limit);
        }
    }
}