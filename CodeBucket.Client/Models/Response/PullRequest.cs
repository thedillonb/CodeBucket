using System;
using System.Collections.Generic;

namespace CodeBucket.Client.V1
{
    public class PullRequestComment
    {
        public string Username { get; set; }
        public string Content { get; set; }
        public string ContentRendered { get; set; }
        public int CommentId { get; set; }
    }
}

namespace CodeBucket.Client
{
    public class PullRequest
    {
		public int Id { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
		public string State { get; set; }
		public string Description { get; set; }
		public string Title { get; set; }
		public bool CloseSourceBranch { get; set; }
        public string Reason { get; set; }
        public Target Source { get; set; }
        public Target Destination { get; set; }
        public List<User> Reviewers { get; set; }
        public User Author { get; set; }
        public List<PullRequestParticipant> Participants { get; set; }
        public PullRequestLinks Links { get; set; }

        public class Target
        {
            public Repository Repository { get; set; }
        }

        public class PullRequestLinks
        {
            public Link Html { get; set; }
        }
    }

    public class PullRequestParticipant
    {
        public string Role { get; set; }
        public User User { get; set; }
        public bool Approved { get; set; }
    }

	public class PullRequestComment
	{
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
		public ulong Id { get; set; }
		public ContentTypes Content { get; set; }
        public InlineDetails Inline { get; set; }
		public User User { get; set; }

		public class ContentTypes
		{
			public string Raw { get; set; }
			public string Markup { get; set; }
			public string Html { get; set; }
		}

        public class InlineDetails
        {
            public int? To { get; set; }
            public int? From { get; set; }
            public string Path { get; set; }
        }
	}
}

