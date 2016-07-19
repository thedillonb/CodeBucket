using CodeBucket.Client.V1;
using CodeBucket.Core.Filters;

namespace CodeBucket.Core.Messages
{
	public class IssueAddMessage
	{
        public Issue Issue { get; }

        public IssueAddMessage(Issue issue)
        {
            Issue = issue;
        }
	}

    public class IssueDeleteMessage
    {
        public Issue Issue { get; }

        public IssueDeleteMessage(Issue issue)
        {
            Issue = issue;
        }
    }

    public class IssueUpdateMessage
    {
        public Issue Issue { get; }

        public IssueUpdateMessage(Issue issue)
        {
            Issue = issue;
        }
    }

    public class IssuesFilterMessage
    {
        public IssuesFilterModel Filter;
    }
}

