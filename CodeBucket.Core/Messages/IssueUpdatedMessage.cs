using System;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
    public class IssueUpdatedMessage
    {
        public string Username { get; }

        public string Repository { get; }

        public int IssueId { get; }

        public IssueModel Issue { get; }

        public IssueUpdatedMessage(string username, string repository, IssueModel issue)
        {
            Username = username;
            Repository = repository;
            IssueId = issue.LocalId;
            Issue = issue;
        }
    }
}

