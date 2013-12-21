using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace CodeBucket.Core.Messages
{
	public class NotificationCountMessage : MvxMessage
	{
		public NotificationCountMessage(object sender) : base(sender) {}
		public int Count;
	}
}

