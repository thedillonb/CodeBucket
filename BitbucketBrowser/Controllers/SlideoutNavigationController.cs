using System;
using MonoTouch.UIKit;
using BitbucketBrowser.Controllers.Events;
using BitbucketBrowser.Data;

namespace BitbucketBrowser.Controllers
{
	public class SlideoutNavigationController : MonoTouch.SlideoutNavigation.SlideoutNavigationController
	{
		private Account _previousUser;

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

			//Double check!
			if (Application.Account == null)
				return;

			//The previous user was returning from the settings menu. Nothing has changed as far as current user goes
			if (Application.Account.Equals(_previousUser))
				return;
			_previousUser = Application.Account;

			//Release the cache
			Application.Cache.DeleteAll();

			//Select a view based on the account type
			if (Application.Account.AccountType == BitbucketBrowser.Data.Account.Type.Bitbucket)
			{
				SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
			}
			else if (Application.Account.AccountType == BitbucketBrowser.Data.Account.Type.GitHub)
			{
				SelectView(new GitHub.Controllers.Events.EventsController(Application.Account.Username, false));
			}
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//This shouldn't happen
			if (Application.Account == null)
				return;

			//Determine which menu to instantiate by the account type!
			if (Application.Account.AccountType == Account.Type.Bitbucket)
			{
				MenuView = new Bitbucket.Controllers.MenuController();
			}
			else if (Application.Account.AccountType == Account.Type.GitHub)
			{
				MenuView = new GitHub.Controllers.MenuController();
			}
		}
	}
}

