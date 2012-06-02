using System;
using MonoTouch.Dialog;


namespace BitbucketBrowser
{
	public class GroupController : DialogViewController
	{
		public GroupController() : base(null, false)
		{
			Style = MonoTouch.UIKit.UITableViewStyle.Plain;
			Root = new RootElement("Groups");
			Root.Add(new Section());
			this.RefreshRequested += (sender, e) => Refresh();
		}
		
		private void Refresh()
		{
			
		}
		
	}
}

