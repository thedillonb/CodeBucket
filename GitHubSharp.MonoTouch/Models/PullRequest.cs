using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GitHubSharp.Models
{
    public class PullRequest
    {
        public DateTime IssueUpdated { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime? Closed { get; set; }
        public int Position { get; set; }
        public int Number { get; set; }
        public int Votes { get; set; }
        public string PatchUrl { get; set; }
        public int Comments { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public IssueUser IssueUser { get; set; }
        public IssueUser User { get; set; }
        public PullRequestCommitReference Base { get; set; }
        public PullRequestCommitReference Head { get; set; }
        string State { get; set; }
        public string[] Labels { get; set; }
        public string DiffUrl { get; set; }
        public List<DiscussionEntry> Discussion { get; set; }
    }

    public class DiscussionEntry
    {
        public string Type { get; set; }
        public DateTime Created { get; set; }
        public string Id { get; set; }
        public string Body { get; set; }
        public string Message { get; set; }
        public IssueUser User { get; set; }
        public IssueUser Author { get; set; }
    }

    public class IssueUser
    {
        public string GravatarId { get; set; }
        public string Type { get; set; }
        public string Login { get; set; }
        public string Name { get; set;}
        public string Email { get; set; }
    }

    public class PullRequestCommitReference
    {
        public Repository Repository { get; set; }
        public string Sha { get; set; }
        public string Label { get; set; }
        public IssueUser User { get; set; }
        public string Ref { get; set; }
    }
}