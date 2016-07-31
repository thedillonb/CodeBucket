using System.Collections.Generic;
using System;

namespace CodeBucket.Client.V1
{
    public class EventCollection
    {
        public int Count { get; set; }
        public List<EventItem> Events { get; set; } 
    }

    public class EventItem
    {
        public string Node { get; set; }
        public object Description { get; set; }
        public Repository Repository { get; set; }
        public User User { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public string Event { get; set; }

        public static class Type
        {
			public const string 

                //Repository
                Commit = "commit", CreateRepo = "create", DeleteRepo = "delete", StripRepo = "strip", ForkRepo = "fork", PatchQueue = "mq", Pushed = "pushed",

                //Wiki
                WikiCreated = "wiki_created", WikiUpdated = "wiki_updated", WikiDeleted = "wiki_deleted",

                //Following
                StartFollowUser = "start_follow_user", StopFollowUser = "stop_follow_user",
                StartFollowRepo = "start_follow_repo", StopFollowRepo = "stop_follow_repo",
                StartFollowIssue = "start_follow_issue", StopFollowIssue = "stop_follow_issue",

                //Issues
                IssueReported = "report_issue", IssueUpdated = "issue_update", IssueComment = "issue_comment",

                //File
                FileUploaded = "file_uploaded", 

                //Pull Requests
                PullRequestCreated = "pullrequest_created", PullRequestUpdated = "pullrequest_updated",
                PullRequestFulfilled = "pullrequest_fulfilled", PullRequestRejected = "pullrequest_rejected",
                PullRequestSuperseded = "pullrequest_superseded", PullRequestCommentCreated = "pullrequest_comment_created",
                PullRequestCommentUpdated = "pullrequest_comment_updated", PullRequestCommentDeleted = "pullrequest_comment_deleted",
                PullRequestLike = "pullrequest_like", PullRequestUnlike = "pullrequest_unlike",

                //ChangeSet
                ChangeSetCommentCreated = "cset_comment_created", ChangeSetCommentUpdated = "cset_comment_updated", 
                ChangeSetCommentDeleted = "cset_comment_deleted",
                ChangeSetLike = "cset_like", ChangeSetUnlike = "cset_unlike";
        }
    }

	public class PushedEventDescriptionModel
	{
		public List<CommitModel> Commits { get; set; }

		public class CommitModel
		{
			public string Hash { get; set; }
			public string Description { get; set; }
		}
	}
}
