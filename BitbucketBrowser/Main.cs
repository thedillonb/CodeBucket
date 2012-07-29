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

            //UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.BlackOpaque;

            //Set the theming
            UINavigationBar.Appearance.SetBackgroundImage(Images.Titlebar, UIBarMetrics.Default);
            UIBarButtonItem.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(15, 6, 15, 6)), UIControlState.Normal, UIBarMetrics.Default);
            UIBarButtonItem.Appearance.SetBackButtonBackgroundImage(Images.BackButton.CreateResizableImage(new UIEdgeInsets(0, 14, 0, 5)), UIControlState.Normal, UIBarMetrics.Default);
            UISegmentedControl.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(15, 6, 14, 6)), UIControlState.Normal, UIBarMetrics.Default);
            UISegmentedControl.Appearance.SetDividerImage(Images.Divider, UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);
            UIToolbar.Appearance.SetBackgroundImage(Images.Bottombar, UIToolbarPosition.Bottom, UIBarMetrics.Default);
            UISearchBar.Appearance.BackgroundImage = Images.Searchbar;

            Application.LoadSettings();

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

            var killSplash = new UIImageView(UIImageHelper.FromFileAuto("Default"));
            window.AddSubview(killSplash);
            window.BringSubviewToFront(killSplash);

            UIView.Animate(0.8, () => { 
                killSplash.Alpha = 0.0f; 
            }, () => { 
                killSplash.RemoveFromSuperview(); 
            });
			
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

            _nav = new SlideoutNavigationController();
            _nav.SetMenuNavigationBackgroundImage(Images.TitlebarDark, UIBarMetrics.Default);
            _nav.MenuView = new MenuController();
            window.RootViewController = _nav;
        }

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            //Pop back to the root view...
            if (_nav.TopView != null && _nav.TopView.NavigationController != null)
                _nav.TopView.NavigationController.PopToRootViewController(false);
        }

	}

    public class MenuController : DialogViewController
    {
        public MenuController()
            : base(UITableViewStyle.Plain, new RootElement("Bucket Browser"))
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

        private class TitleView : UIView
        {
            public string Name
            {
                get { return _name.Text; }
                set 
                { 
                    _name.Text = value;
                    _name.SetNeedsDisplay();
                }
            }

            private UILabel _name;

            public TitleView()
                : base(new RectangleF(0, 0, 200, 44))
            {
                this.AutosizesSubviews = true;


                var l = new UILabel(new RectangleF(0, 5, Frame.Width, 20));
                l.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                l.BackgroundColor = UIColor.Clear;
                l.Font = UIFont.BoldSystemFontOfSize(18f);
                l.ShadowColor = UIColor.FromWhiteAlpha(0, 0.5f);
                l.TextColor = UIColor.White;
                l.Text = "Bucket Browser";
                l.TextAlignment = UITextAlignment.Left;
                this.Add(l);

                _name = new UILabel(new RectangleF(0, 24, Frame.Width, 14));
                _name.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
                _name.BackgroundColor = UIColor.Clear;
                _name.Font = UIFont.BoldSystemFontOfSize(10f);
                _name.ShadowColor = UIColor.FromWhiteAlpha(0, 0.5f);
                _name.TextColor = UIColor.White;
                _name.Text = "";
                _name.TextAlignment = UITextAlignment.Left;
                this.Add(_name);
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

            //_titleView = new TitleView();
            //NavigationItem.TitleView = _titleView;
            //_titleView.SizeToFit();
            //_titleView.Name = "thedillonb";

            var username = Application.Account.Username;
            Title = username;

            Root.Add(new Section() {
                new NavElement("Profile", () => DoShit(new ProfileController(username, false) { Title = "Profile" }), Images.Person),
                new NavElement("Events", () => DoShit(new EventsController(username, false) { Title = "Events", ReportRepository = true }), Images.Event),
                new NavElement("Repositories", () => DoShit(new AccountRepositoryController(username) { Title = "Repositories" }), Images.Repo),
                new NavElement("Groups", () => DoShit(new GroupController(username, false) { Title = "Groups" }), Images.Group),
                new NavElement("Explore", () => DoShit(new ExploreController() { Title = "Explore" }), UIImage.FromBundle("/Images/Tabs/search")),
            });

            /*
            Root.Add(new Section("Settings") {
                new NavElement("Login", () => { 
                    PresentModalViewController(new LoginViewController(), true);
                
                }, UIImage.FromBundle("/Images/Tabs/person"))
            });
            */

            TableView.BackgroundColor = UIColor.Clear;
            UIImage background = Images.Linen;
            View.BackgroundColor = UIColor.FromPatternImage(background);

            TableView.SeparatorColor = UIColor.FromRGBA(128, 128, 128, 128);

            var view = new UIView(new RectangleF(0, 0, View.Bounds.Width, 10));
            view.BackgroundColor = UIColor.Clear;
            TableView.TableFooterView = view;
        }

        private string _previousUser;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            //Check to see if anything changed!
            //_titleView.Name = Application.Account.Username;
            Title = Application.Account.Username;

            //First time appear
            if (_previousUser == null)
            {
                NavigationController.PushViewController(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true }, false);
                _previousUser = Application.Account.Username;
            }
        }


        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (!(_previousUser ?? "").Equals(Application.Account.Username))
                NavigationController.PushViewController(new EventsController(Application.Account.Username, false) { Title = "Events", ReportRepository = true }, false);
            _previousUser = Application.Account.Username;
        }
    }
}

