using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Events
{
    public class UserEventsViewModel : BaseEventsViewModel
    {
        private readonly IApplicationService _applicationService;

        public string Username { get; private set; }

        public UserEventsViewModel(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

        protected override async Task Load()
        {
            var events = await _applicationService.Client.Users.GetEvents(Username);
            Events.Items.Reset(CreateDataFromLoad(events.Events));
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}
