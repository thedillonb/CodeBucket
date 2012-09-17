using System;

namespace GitHubSharp.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Body { get; set; }
        public BasicUser User { get; set; }
        public DateTime created_at { get; set; }
    }

    public class CommentForCreationOrEdit
    {
        public string Body { get; set; }
    }
}

