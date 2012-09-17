using System;

namespace GitHubSharp.Models
{
    public class Issue
    {
        public int Number { get; set; }
        public int Votes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public DateTime? ClosedAt { get; set; }
        public string User { get; set; }
        public string[] Labels { get; set; }                
        public string State { get; set; }
        public int Comments { get; set; }
        public double Position { get; set; }
        public string HtmlUrl { get; set; }
        
    }

    public enum IssueState
    {
        Open,
        Closed
    }

    public class CommentForIssue
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Body { get; set; }
        public int Id { get; set; }
        public string User { get; set; }
    }
}
