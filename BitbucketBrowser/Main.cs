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
using MonoTouch.Dialog.Utilities;
using CodeFramework.UI.Elements;
using CodeFramework.UI.Views;
using BitbucketBrowser.UI.Controllers.Issues;
using BitbucketBrowser.UI.Controllers.Events;
using BitbucketBrowser.UI.Controllers.Repositories;
using BitbucketBrowser.UI.Controllers.Groups;
using BitbucketBrowser.UI.Controllers.Accounts;
using BitbucketBrowser.UI.Controllers.Changesets;

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
            //Set the theming
            UINavigationBar.Appearance.SetBackgroundImage(Images.Titlebar.CreateResizableImage(new UIEdgeInsets(0, 0, 1, 0)), UIBarMetrics.Default);
            UIBarButtonItem.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(12, 6, 11, 6)), UIControlState.Normal, UIBarMetrics.Default);
            UIBarButtonItem.Appearance.SetBackButtonBackgroundImage(Images.BackButton.CreateResizableImage(new UIEdgeInsets(0, 14, 0, 5)), UIControlState.Normal, UIBarMetrics.Default);
            UISegmentedControl.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(12, 6, 11, 6)), UIControlState.Normal, UIBarMetrics.Default);
            UISegmentedControl.Appearance.SetDividerImage(Images.Divider, UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);
            UIToolbar.Appearance.SetBackgroundImage(Images.Bottombar.CreateResizableImage(new UIEdgeInsets(0, 0, 0, 0)), UIToolbarPosition.Bottom, UIBarMetrics.Default);
            UISearchBar.Appearance.BackgroundImage = Images.Searchbar;

            var textAttrs = new UITextAttributes() { TextColor = UIColor.White, TextShadowColor = UIColor.DarkGray, TextShadowOffset = new UIOffset(0, -1) };
            UINavigationBar.Appearance.SetTitleTextAttributes(textAttrs);
            UISegmentedControl.Appearance.SetTitleTextAttributes(textAttrs, UIControlState.Normal);

            DropbarView.Image = UIImage.FromBundle("/Images/Dropbar");
            WatermarkView.Image = Images.Background;
            HeaderView.Gradient = Images.CellGradient;
            StyledElement.BgColor = UIColor.FromPatternImage(Images.TableCell);
            ErrorView.AlertImage = UIImage.FromBundle("/Images/warning.png");

            //Resize the back button only on the iPhone
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                UIBarButtonItem.Appearance.SetBackButtonBackgroundImage(Images.BackButtonLandscape.CreateResizableImage(new UIEdgeInsets(0, 12, 0, 5)), UIControlState.Normal, UIBarMetrics.LandscapePhone);
            }

            window = new UIWindow(UIScreen.MainScreen.Bounds);

            if (Application.Accounts.Count == 0)
            {
                var login = new LoginViewController();
                login.LoginComplete = () => {
                    ShowMainWindow();
                };

                //Make it so!
                window.RootViewController = login;
            }
            else
            {
                ShowMainWindow();
            }

			window.MakeKeyAndVisible();


            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                var killSplash = new UIImageView(UIImageHelper.FromFileAuto("Default"));
                window.AddSubview(killSplash);
                window.BringSubviewToFront(killSplash);

                UIView.Animate(0.8, () => { 
                    killSplash.Alpha = 0.0f; 
                }, () => { 
                    killSplash.RemoveFromSuperview(); 
                });
            }


			
			return true;
		}

        private void ShowMainWindow()
        {
            var defaultAccount = Application.Accounts.GetDefault();
            if (defaultAccount == null)
            {
                defaultAccount = Application.Accounts.First();
                Application.Accounts.SetDefault(defaultAccount);
            }

            Application.SetUser(defaultAccount);

            _nav = new MySlideout() { SlideHeight = 999f };
            _nav.SetMenuNavigationBackgroundImage(Images.TitlebarDark, UIBarMetrics.Default);
            _nav.MenuView = new MenuController();
            window.RootViewController = _nav;
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            //Remove everything from the cache
            Application.Cache.DeleteAll();

            //Pop back to the root view...
            if (_nav.TopView != null && _nav.TopView.NavigationController != null)
                _nav.TopView.NavigationController.PopToRootViewController(false);
        }
	}

    public class MySlideout : SlideoutNavigationController
    {
        private string _previousUser;


        public MySlideout()
            : base()
        {
        }
        
        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!(_previousUser ?? "").Equals(Application.Account.Username))
                SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
            _previousUser = Application.Account.Username;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //First time appear
            if (_previousUser == null)
            {
#if DEBUG
                SelectView(new IssuesController(Application.Account.Username, "bitbucketsharp") { Title = "Events" });
#else
                SelectView(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true });
#endif
                _previousUser = Application.Account.Username;
            }

        }
    }

    public class MenuController : DialogViewController
    {
        public MenuController()
            : base(UITableViewStyle.Plain, new RootElement("CodeBucket"))
        {
            if (Application.Account != null && !string.IsNullOrEmpty(Application.Account.Username))
                Root.Caption = Application.Account.Username;
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

        private void DoShit(UIViewController controller)
        {
            NavigationController.PushViewController(controller, false);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Images.ChangeUser, UIBarButtonItemStyle.Plain, (s, e) => {
                var n = new UINavigationController(new MyAccountsController());
                this.PresentModalViewController(n, true);
            
            });

            Root.Add(new Section() {
                new NavElement("Profile", () => DoShit(new ProfileController(Application.Account.Username, false) { Title = "Profile" }), Images.Person),
                new NavElement("Events", () => DoShit(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true }), Images.Event),
                new NavElement("Repositories", () => DoShit(new AccountRepositoryController(Application.Account.Username) { Title = "Repositories" }), Images.Repo),
                new NavElement("Groups", () => DoShit(new GroupController(Application.Account.Username, false) { Title = "Groups" }), Images.Group),
                new NavElement("Explore", () => DoShit(new ExploreController() { Title = "Explore" }), UIImage.FromBundle("/Images/Tabs/search")),
            });

            TableView.BackgroundColor = UIColor.Clear;
            UIImage background = Images.Linen;
            View.BackgroundColor = UIColor.FromPatternImage(background);

            TableView.SeparatorColor = UIColor.FromRGBA(128, 128, 128, 128);

            var view = new UIView(new RectangleF(0, 0, View.Bounds.Width, 10));
            view.BackgroundColor = UIColor.Clear;
            TableView.TableFooterView = view;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            Root.Caption = Application.Account.Username;
            Title = Root.Caption;
        }

    }
}

