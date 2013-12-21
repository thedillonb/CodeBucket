using System;
using Cirrious.MvvmCross.Plugins.Messenger;
using BitbucketSharp.Models;

namespace CodeBucket.Core.Messages
{
	public class SelectedMilestoneMessage : MvxMessage
	{
		public SelectedMilestoneMessage(object sender) : base(sender) {}
		public MilestoneModel Milestone;
	}
}

