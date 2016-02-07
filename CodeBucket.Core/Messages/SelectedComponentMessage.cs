using MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.Messages
{
    public class SelectedComponentMessage : MvxMessage
	{
        public SelectedComponentMessage(object sender) : base(sender) {}
        public string Value;
	}
}

