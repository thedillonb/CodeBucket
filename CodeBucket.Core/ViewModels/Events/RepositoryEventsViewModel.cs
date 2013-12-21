using System.Collections.Generic;
using BitbucketSharp.Models;
using System.Linq;

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

		protected override int GetTotalItemCount()
		{
			return this.GetApplication().Client.Users[Username].Repositories[Repository].GetEvents(0, 0).Count;
		}

		protected override List<EventModel> CreateRequest(int start, int limit)
        {
			var events = this.GetApplication().Client.Users[Username].Repositories[Repository].GetEvents(start, limit);
			return events.Events.OrderByDescending(x => x.UtcCreatedOn).ToList();
        }

        public class NavObject
        {
            public string Username { get; set; }
            public string Repository { get; set; }
        }
    }
}