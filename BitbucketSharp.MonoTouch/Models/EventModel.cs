using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class EventsModel
    {
        public int Count { get; set; }
        public List<EventModel> Events { get; set; } 
    }

    public class EventModel
    {
        public string Node { get; set; }
        public string Description { get; set; }
        public RepositoryDetailedModel Repository { get; set; }
        public string CreatedOn { get; set; }
        public UserModel User { get; set; }
        public string UtcCreatedOn { get; set; }
        public string Event { get; set; }
		
		
		public static IDictionary<string, string> EventToString = new Dictionary<string, string>() {
			{"commit", "Commit"}, {"wiki_updated", "Wiki Update"}, {"wiki_created", "Wiki Create"},
			{"start_follow_user", "Following User"}
		};
    }
}
