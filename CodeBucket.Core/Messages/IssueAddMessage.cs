using System;
using BitbucketSharp.Models;
using MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.Messages
{
	public class IssueAddMessage : MvxMessage
	{
		public IssueAddMessage(object sender) : base(sender) {}
		public IssueModel Issue;
	}
}

