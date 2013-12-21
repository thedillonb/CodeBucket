using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
	public class IssueAddMessage : MvxMessage
	{
		public IssueAddMessage(object sender) : base(sender) {}
		public IssueModel Issue;
	}
}

