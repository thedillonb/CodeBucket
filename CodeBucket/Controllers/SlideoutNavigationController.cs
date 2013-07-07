using System;
using MonoTouch.UIKit;
using CodeBucket.Data;

namespace CodeBucket.Controllers
{
	public class SlideoutNavigationController : MonoTouch.SlideoutNavigation.SlideoutNavigationController
	{
		private Account _previousUser;

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeBucket.Controllers.SlideoutNavigationController"/> class.
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
			SelectView(new CodeBucket.Bitbucket.Controllers.Events.EventsController(Application.Account.Username, false) { ReportRepository = true });
		}
		
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//This shouldn't happen
			if (Application.Account == null)
				return;

			//Determine which menu to instantiate by the account type!
			MenuView = new Bitbucket.Controllers.MenuController();
		}
	}
}

