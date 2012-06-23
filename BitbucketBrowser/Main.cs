using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using BitbucketBrowser.UI;
using MonoTouch.Dialog;
using MonoTouch.SlideoutNavigation;
using System.Drawing;

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
        SlideoutNavigationController _nav;

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
            Application.Client = new BitbucketSharp.Client("thedillonb", "djames");


			window = new UIWindow(UIScreen.MainScreen.Bounds);
            _nav = new SlideoutNavigationController();

            _nav.MenuView = new MenuController();
            _nav.TopView = new EventsController("thedillonb", false) { Title = "Events", ReportUser = false };
			
			window.RootViewController = _nav;
			window.MakeKeyAndVisible();
			
			return true;
		}
	}

    /// <summary>
    /// Application.
    /// </summary>
    public static class Application
    {
        public static BitbucketSharp.Client Client { get; set; }
    }

    public class MenuController : DialogViewController
    {
        public MenuController()
            : base(UITableViewStyle.Plain, new RootElement(""))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Root.Add(new Section() {
                new ImageStringElement("Profile", () => NavigationController.PushViewController(new ProfileController("thedillonb", false) { Title = "Profile" }, false), UIImage.FromBundle("/Images/Tabs/user.png")),
                new ImageStringElement("Events", () => NavigationController.PushViewController(new EventsController("thedillonb", false) { Title = "Events", ReportUser = false }, false), UIImage.FromBundle("/Images/Tabs/events.png")),
                new ImageStringElement("Repositories", () => NavigationController.PushViewController(new AccountRepositoryController("thedillonb") { Title = "Repositories" }, false), UIImage.FromBundle("/Images/Tabs/database.png")),
                new ImageStringElement("Groups", () => NavigationController.PushViewController(new GroupController("thedillonb", false) { Title = "Groups" }, false), UIImage.FromBundle("/Images/Tabs/groups.png")),
                new ImageStringElement("Explore", () => NavigationController.PushViewController(new ExploreController() { Title = "Explore" }, false), UIImage.FromBundle("/Images/Tabs/search.png")),
            });
            Root.Add(new Section("Preferences") {
                new ImageStringElement("Help", UIImage.FromBundle("/Images/Tabs/help.png")),
                new ImageStringElement("Settings", UIImage.FromBundle("/Images/Tabs/cog.png")),
                new ImageStringElement("Logout", UIImage.FromBundle("/Images/Tabs/logout.png")),
            });

            NavigationItem.TitleView = new LogoView();
        }

        private class LogoView : UIView
        {
            private static UIImage Logo = UIImage.FromBundle("/Images/bitbucketlogo.png");

            public LogoView()
                : base(new RectangleF(0f, 0f, 40f, 40f))
            {
                BackgroundColor = UIColor.Clear;
            }

            public override void Draw(System.Drawing.RectangleF rect)
            {
                base.Draw(rect);

                //Draw the logo
                Logo.Draw(new RectangleF(0, 0, Logo.Size.Width, Logo.Size.Height));
            }
        }
    }
}

