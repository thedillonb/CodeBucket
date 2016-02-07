using System;
using MvvmCross.Plugins.Messenger;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
	public class IssueEditMessage : MvxMessage
	{
		public IssueEditMessage(object sender) : base(sender) {}
		public IssueModel Issue;
	}
}

