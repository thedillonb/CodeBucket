using CodeBucket.Core.Services;
using System.Threading.Tasks;
using Splat;

namespace CodeBucket.Core.ViewModels.Events
{
    public class UserEventsViewModel : BaseEventsViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly string _username;

        public UserEventsViewModel(string username, IApplicationService applicationService = null)
        {
            _username = username;
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
        }

        protected override Task<BitbucketSharp.Models.EventsModel> GetEvents(int start, int limit)
        {
            return _applicationService.Client.Users.GetEvents(_username, start, limit);
        }
    }
}
