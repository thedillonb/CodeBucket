using System;
using MonoTouch.Dialog;

namespace BitbucketBrowser.UI
{
	public class ProfileController : DialogViewController
	{
		public ProfileController() : base(null, false)
		{
			Style = MonoTouch.UIKit.UITableViewStyle.Grouped;
		}
	}
}

