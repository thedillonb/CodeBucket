using System;
using MonoTouch.Dialog;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Linq;

namespace BitbucketBrowser.UI
{
	public class NewsFeedController : DialogViewController
	{
		private readonly RootElement _newsElement;
		private bool _loaded = false;
		private DateTime _lastUpdate = DateTime.MinValue;

		public NewsFeedController() : base(null, false)
		{
			Style = MonoTouch.UIKit.UITableViewStyle.Plain;
			
			_newsElement = new RootElement("News") { UnevenRows = true };
			Root = _newsElement;
			Root.Add(new Section());
			this.RefreshRequested += (sender, e) => Refresh();
		}
		
		private void Refresh()
		{
			ThreadPool.QueueUserWorkItem((obj) => {
				BitbucketSharp.Client client = new BitbucketSharp.Client("thedillonb", "djames");
				var events = client.Account.GetEvents();

				var newEvents = (from s in events.Events where DateTime.Parse(s.CreatedOn) > _lastUpdate orderby DateTime.Parse(s.CreatedOn) select s).ToList();
				if (newEvents.Count > 0)
					_lastUpdate = (from r in newEvents select DateTime.Parse(r.CreatedOn)).Max();

				InvokeOnMainThread(delegate {
					var sec = Root[0];
					foreach (var e in newEvents)
						sec.Insert(0, UITableViewRowAnimation.Top,new NewsFeedElement(e));
					ReloadComplete();
				});
			});
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			if (!_loaded) 
			{
				Refresh();
				_loaded = true;
			}
		}
	}
}

