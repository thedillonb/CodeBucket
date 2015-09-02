using System;

namespace CodeBucket.Core.ViewModels.Issues
{
	//        private static readonly string[] Priorities = { "Trivial", "Minor", "Major", "Critical", "Blocker" };
//        private static readonly string[] Statuses = { "New", "Open", "Resolved", "On Hold", "Invalid", "Duplicate", "Wontfix" };
//        private static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };

	public class IssueKindViewModel : BaseViewModel
    {
		private static readonly string[] Kinds = { "Bug", "Enhancement", "Proposal", "Task" };

		public string[] Values 
		{
			get { return Kinds; }
		}
    }
}

