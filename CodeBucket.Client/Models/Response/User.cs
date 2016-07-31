using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace CodeBucket.Client.V1
{
    public class Followers
    {
        public int Count { get; set; }

        [JsonProperty(PropertyName = "followers")]
        public List<User> Users { get; set; }
    }

    public class User
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsTeam { get; set; }
        public string Avatar { get; set; }
        public string ResourceUrl { get; set; }
    }
}

namespace CodeBucket.Client
{
    public class User
    {
        public string Username { get; set; }
        public string Type { get; set; }
        public string Website { get; set; }
        public string DisplayName { get; set; }
        public string Location { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public UserLinks Links { get; set; }

        public class UserLinks
        {
            public Link Avatar { get; set; }
        }
    }
}
