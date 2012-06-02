using System;
using MonoTouch.Dialog;
using System.Threading;

namespace BitbucketBrowser.UI
{
	public class RepositoryController : DialogViewController
	{
		private readonly RootElement _root;
		private bool loaded = false;
		
		public RepositoryController() : base(null, false)
		{
			_root = new RootElement("Repositories");
			Root = _root;
			Style = MonoTouch.UIKit.UITableViewStyle.Plain;
			RefreshRequested += (sender, e) => Refresh();
		}
		
		private void Refresh()
		{
			ThreadPool.QueueUserWorkItem((obj) => {
				BitbucketSharp.Client client = new BitbucketSharp.Client("thedillonb", "djames");
				var repos = client.Account.GetRepositories();
				var sec = new Section();
			
				repos.ForEach(e => {
					sec.Add(new StringElement(
						e.Name,
						() => NavigationController.PushViewController(new RepositoryInfoController(e), true)
					));
				}
				);
				
				
				InvokeOnMainThread(delegate {
					this.Root.Clear();
					this.Root.Add(sec);
					
					ReloadComplete();
				}
				);
			}
			);
		}
		
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (!loaded)
			{
				Refresh();
				loaded = true;
			}
		}
	}
}

