using System;
using MvvmCross.Plugins.Messenger;
using CodeBucket.Client.Models;

namespace CodeBucket.Core.Messages
{
	public class SelectedAssignedToMessage : MvxMessage
	{
		public SelectedAssignedToMessage(object sender) : base(sender) {}
		public UserModel User;
	}
}

