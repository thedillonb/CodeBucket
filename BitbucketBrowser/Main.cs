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
		LoginViewController _loginController;
		RepositoryController _repoController;
		NewsFeedController _newsController;
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
			_loginController = new LoginViewController();
			_repoController = new RepositoryController();
			_newsController = new NewsFeedController();
			_groupController = new GroupController();
			_profileController = new ProfileController();
			_tabs = new UITabBarController();
			
			_tabs.ViewControllers = new UIViewController[] {
				new UINavigationController(_newsController) { TabBarItem = new UITabBarItem("News", UIImage.FromBundle("Images/newspaper.png"), 1) },
				new UINavigationController(_repoController) { TabBarItem = new UITabBarItem("Repositories", UIImage.FromBundle("Images/database.png"), 2) },
				new UINavigationController(_groupController) { TabBarItem = new UITabBarItem("Groups", UIImage.FromBundle("Images/people_family.png"), 3) },
				new UINavigationController(_profileController) { TabBarItem = new UITabBarItem("Profile", UIImage.FromBundle("Images/111-user.png"), 4) }
			};
			
			window.RootViewController = _tabs;
			window.MakeKeyAndVisible();
			
			return true;
		}
	}
}

