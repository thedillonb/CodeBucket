using System.Threading.Tasks;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.ViewModels.Events
{
    public class UserEventsViewModel : BaseEventsViewModel
    {
        public string Username
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
        }

		protected override Task<EventsModel> CreateRequest(int start, int limit)
        {
            return this.GetApplication().Client.Users.GetEvents(Username, start, limit);
        }

        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}
