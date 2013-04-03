using System;
using MonoTouch.UIKit;
using BitbucketBrowser.Controllers.Events;

namespace BitbucketBrowser.Controllers
{
	public class SlideoutNavigationController : MonoTouch.SlideoutNavigation.SlideoutNavigationController
	{
		private string _previousUser;

		/// <summary>
		/// Initializes a new instance of the <see cref="BitbucketBrowser.Controllers.SlideoutNavigationController"/> class.
		/// </summary>
		public SlideoutNavigationController()
		{
			//Setting the height to a large amount means that it will activate the slide pretty much whereever your finger is on the screen.
			SlideHeight = 999f;
		}

		/// <summary>
		/// Creates the menu button.
		/// </summary>
		/// <returns>The menu button.</returns>
		protected override UIBarButtonItem CreateMenuButton()
		{
			return new UIBarButtonItem(Images.ThreeLines, UIBarButtonItemStyle.Plain, (s, e) => Show());
		}
		
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (!(_previousUser ?? "").Equals(Application.Account.Username))
			{
				Application.Cache.DeleteAll();
				SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
			}
			_previousUser = Application.Account.Username;
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			
			//First time appear
			if (_previousUser == null)
			{
				#if DEBUG
				//SelectView(new IssuesController(Application.Account.Username, "bitbucketbrowser"));
				SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
				#else
				SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
				#endif
				Application.Cache.DeleteAll();
				_previousUser = Application.Account.Username;
			}
			
		}
	}
}

