using System;
using MvvmCross.Plugins.Messenger;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
    public class IssueDeleteMessage : MvxMessage
	{
        public IssueDeleteMessage(object sender) : base(sender) {}
		public IssueModel Issue;
	}
}

