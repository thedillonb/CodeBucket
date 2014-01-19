using Cirrious.MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.Messages
{
	public class SelectedMilestoneMessage : MvxMessage
	{
		public SelectedMilestoneMessage(object sender) : base(sender) {}
        public string Value;
	}
}

