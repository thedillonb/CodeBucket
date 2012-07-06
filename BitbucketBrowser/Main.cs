using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using BitbucketBrowser.UI;
using MonoTouch.Dialog;
using MonoTouch.SlideoutNavigation;
using System.Drawing;
using System.Threading;

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
            _nav.TopView = new ChangesetInfoController("thedillonb", "bitbucketsharp", "e9d8cf73c610"); //new EventsController("thedillonb", false) { Title = "Events", ReportUser = false };

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
            : base(UITableViewStyle.Plain, new RootElement("BitbucketBrowser"))
        {
        }

        class NavElement : StyledElement
        {
            public NavElement(string title, NSAction tapped, UIImage image)
                : base(title, tapped, image)
            {
                BackgroundColor = UIColor.Clear;
                TextColor = UIColor.White;
                DetailColor = UIColor.White;
            }


            public override UITableViewCell GetCell(UITableView tv)
            {
                var cell = base.GetCell(tv);
                cell.SelectedBackgroundView = new UIView() { BackgroundColor = UIColor.FromRGBA(41, 41, 41, 200) };

                var f = cell.Subviews.Count(x => x.Tag == 1111);
                if (f == 0)
                {

                    var v2 = new UIView(new RectangleF(0, cell.Frame.Height - 3, cell.Frame.Width, 1));
                    v2.BackgroundColor = UIColor.FromRGBA(41, 41, 41, 64);
                    v2.Tag = 1111;
                    cell.AddSubview(v2);


                    var v = new UIView(new RectangleF(0, cell.Frame.Height - 2, cell.Frame.Width, 1));
                    v.BackgroundColor = UIColor.FromRGBA(41, 41, 41, 200);
                    v.Tag = 1111;
                    cell.AddSubview(v);
                }

                return cell;
            }

        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationController.SetNavigationBarHidden(true, false);

            Root.Add(new Section() {
                new NavElement("Profile", () => NavigationController.PushViewController(new ProfileController("thedillonb", false) { Title = "Profile" }, false), UIImage.FromBundle("/Images/Tabs/person")),
                new NavElement("Events", () => NavigationController.PushViewController(new EventsController("thedillonb", false) { Title = "Events", ReportUser = false, ReportRepository = true }, false), UIImage.FromBundle("/Images/Tabs/events")),
                new NavElement("Repositories", () => NavigationController.PushViewController(new AccountRepositoryController("thedillonb") { Title = "Repositories" }, false), UIImage.FromBundle("/Images/repo")),
                new NavElement("Groups", () => NavigationController.PushViewController(new GroupController("thedillonb", false) { Title = "Groups" }, false), UIImage.FromBundle("/Images/Tabs/group")),
                new NavElement("Explore", () => NavigationController.PushViewController(new ExploreController() { Title = "Explore" }, false), UIImage.FromBundle("/Images/Tabs/search")),
            });

            /*
            Root.Add(new Section("Settings") {
                new NavElement("Login", () => { 
                    PresentModalViewController(new LoginViewController(), true);
                
                }, UIImage.FromBundle("/Images/Tabs/person"))
            });
            */

            TableView.BackgroundColor = UIColor.Clear;
            UIImage background = UIImage.FromBundle("/Images/Cells/background2");
            View.BackgroundColor = UIColor.FromPatternImage(background);

            TableView.SeparatorColor = UIColor.FromRGBA(128, 128, 128, 128);


            /*
            Root.Add(new Section("Settings") {
                new CustomImageStringElement("Change User", () => NavigationController.PushViewController(new ProfileController("thedillonb", false) { Title = "Profile" }, false), UIImage.FromBundle("/Images/Tabs/person")),
            });
            */

            var view = new UIView(new RectangleF(0, 0, View.Bounds.Width, 10));
            view.BackgroundColor = UIColor.Clear;
            TableView.TableFooterView = view;

            /*
            var viewTop = new UIView(new RectangleF(0, 0, View.Bounds.Width, 1));
            viewTop.BackgroundColor = UIColor.LightGray;
            TableView.TableHeaderView = viewTop;
            */

            //NavigationItem.TitleView = new LogoView();
        }
    }
}

