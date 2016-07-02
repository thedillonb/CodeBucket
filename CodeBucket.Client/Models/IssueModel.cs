using System.Collections.Generic;
using System;

namespace CodeBucket.Client.Models
{
    public class IssuesModel
    {
        public int Count { get; set; }
        public string Search { get; set; }
        public List<IssueModel> Issues { get; set; } 
    }

    public class CreateIssueModel
    {
        public string Status { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Responsible { get; set; }
        public string Content { get; set; }
        public string Kind { get; set; }
        public string Component { get; set; }
        public string Milestone { get; set; }
        public string Version { get; set; }

        public bool CheckNoChange()
        {
            return Status == null && Priority == null && Title == null && Responsible == null && Content == null && 
                   Kind == null && Component == null && Milestone == null && Version == null;
        }
    }

    public class IssueModel
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
        public DateTimeOffset UtcUpdatedOn { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public bool IsSpam { get; set; }
    }

    public class ComponentModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class VersionModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    public class MilestoneModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
