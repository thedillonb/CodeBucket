using System.Collections.Generic;
using BitbucketSharp.Models;

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

		protected override List<EventModel> CreateRequest(int start, int limit)
        {
			return this.GetApplication().Client.Users[Username].GetEvents(start, limit).Events;
        }

		protected override int GetTotalItemCount()
		{
			return this.GetApplication().Client.Users[Username].GetEvents(0, 0).Count;
		}


        public class NavObject
        {
            public string Username { get; set; }
        }
    }
}
