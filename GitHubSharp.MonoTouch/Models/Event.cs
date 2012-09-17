using System;

namespace GitHubSharp.Models
{
    public class Event
    {
        public string Type { get; set; }
        public bool Public { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Id { get; set; }
        public BasicUser Actor { get; set; }
        public BasicUser Org { get; set; }
    }
}

