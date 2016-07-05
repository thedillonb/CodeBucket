using CodeBucket.Client.V1;
using CodeBucket.Core.Filters;

namespace CodeBucket.Core.Messages
{
	public class IssueAddMessage
	{
		public Issue Issue;
	}

    public class IssueDeleteMessage
    {
        public Issue Issue;
    }

    public class IssueUpdateMessage
    {
        public string Username;
        public string Repository;
        public Issue Issue;
    }

    public class IssuesFilterMessage
    {
        public IssuesFilterModel Filter;
    }
}

