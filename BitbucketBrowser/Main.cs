using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using BitbucketBrowser.UI;

namespace BitbucketBrowser
{
	
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		AccountRepositoryController _repoController;
		EventsController _newsController;
		GroupController _groupController;
		ProfileController _profileController;
		UITabBarController _tabs;

		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
		
		//
		// This method is invoked when the application has loaded and is ready to run. In this 
		// method you should instantiate the window, load the UI into it and then make the window
		// visible.
		//
		// You have 17 seconds to return from this method, or iOS will terminate your application.
		//
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			window = new UIWindow(UIScreen.MainScreen.Bounds);
            _repoController = new AccountRepositoryController("thedillonb") { Title = "Repositories" };
            _newsController = new EventsController("thedillonb", false) { Title = "Events" };
            _groupController = new GroupController("thedillonb", false) { Title = "Groups" };
            _profileController = new ProfileController("thedillonb", false) { Title = "Profile" };
			_tabs = new UITabBarController();
			
			_tabs.ViewControllers = new UIViewController[] {
				new UINavigationController(_newsController) { TabBarItem = new UITabBarItem(_newsController.Title, UIImage.FromBundle("Images/Tabs/newspaper.png"), 1) },
				new UINavigationController(_repoController) { TabBarItem = new UITabBarItem(_repoController.Title, UIImage.FromBundle("Images/Tabs/database.png"), 2) },
				new UINavigationController(_groupController) { TabBarItem = new UITabBarItem(_groupController.Title, UIImage.FromBundle("Images/Tabs/people_family.png"), 3) },
				new UINavigationController(_profileController) { TabBarItem = new UITabBarItem(_profileController.Title, UIImage.FromBundle("Images/Tabs/111-user.png"), 4) }
			};
			
			window.RootViewController = _tabs;
			window.MakeKeyAndVisible();
			
			return true;
		}
	}
}

