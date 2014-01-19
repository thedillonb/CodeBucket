using Cirrious.MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.Messages
{
    public class SelectedVersionMessage : MvxMessage
	{
        public SelectedVersionMessage(object sender) : base(sender) {}
        public string Value;
	}
}

