using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
	public class SelectedAssignedToMessage : MvxMessage
	{
		public SelectedAssignedToMessage(object sender) : base(sender) {}
		public UserModel User;
	}
}

