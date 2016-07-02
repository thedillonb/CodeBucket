using CodeBucket.Client.Models;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Events
{
    public class RepositoryEventsViewModel : BaseEventsViewModel
    {
        public string Repository 
        { 
            get; 
            private set; 
        }

        public string Username
        {
            get;
            private set;
        }

        public void Init(NavObject navObject)
        {
            Username = navObject.Username;
            Repository = navObject.Repository;
        }

		protected override Task<EventsModel> CreateRequest(int start, int limit)
        {
            return this.GetApplication().Client.Repositories.GetEvents(Username, Repository, start, limit);
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}