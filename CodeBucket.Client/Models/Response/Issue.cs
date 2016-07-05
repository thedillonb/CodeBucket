using System.Collections.Generic;
using System;

namespace CodeBucket.Client.V1
{
    public class IssueCollection
    {
        public int Count { get; set; }
        public string Search { get; set; }
        public List<Issue> Issues { get; set; } 
    }

    public class Issue
    {
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public int CommentCount { get; set; }
        public string Content { get; set; }
        public string CreatedOn { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public DateTimeOffset UtcLastUpdated { get; set; }
        public int LocalId { get; set; }
        public int FollowerCount { get; set; }
        public string ResourceUri { get; set; }
        public bool IsSpam { get; set; }
        public User ReportedBy { get; set; }
        public User Responsible { get; set; }
        public MetaModel Metadata { get; set; }

        public class MetaModel
        {
            public string Kind { get; set; }
            public string Version { get; set; }
            public string Component { get; set; }
            public string Milestone { get; set; }
        }
    }

    public class IssueComment
    {
        public string Content { get; set; }
        public User AuthorInfo { get; set; }
        public int CommentId { get; set; }
        public DateTimeOffset UtcUpdatedOn { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public bool IsSpam { get; set; }
    }

    public class IssueComponent
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class IssueVersion
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class IssueMilestone
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
