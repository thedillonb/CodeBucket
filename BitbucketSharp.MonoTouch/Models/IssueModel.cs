using System.Collections.Generic;

namespace BitbucketSharp.Models
{
    public class IssuesModel
    {
        public int Count { get; set; }
        public string Search { get; set; }
        public List<IssueModel> Issues { get; set; } 
    }

    public class IssueModel
    {
        public string Status { get; set; }
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public string Content { get; set; }
        public string CreatedOn { get; set; }
        public int LocalId { get; set; }
        public int FollowerCount { get; set; }
        public string ResourceUri { get; set; }
        public bool IsSpam { get; set; }
        public UserModel ReportedBy { get; set; }
        public UserModel Responsible { get; set; }
        public MetaModel Metadata { get; set; }

        public class MetaModel
        {
            public string Kind { get; set; }
            public string Version { get; set; }
            public string Component { get; set; }
            public string Milestone { get; set; }
        }
    }

    public class CommentModel
    {
        public string Content { get; set; }
        public UserModel AuthorInfo { get; set; }
        public int CommentId { get; set; }
        public string UtcUpdatedOn { get; set; }
        public string UtcCreatedOn { get; set; }
        public bool IsSpam { get; set; }
    }
}
