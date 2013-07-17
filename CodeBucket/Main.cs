using System.Linq;
using CodeBucket.Bitbucket.Controllers.Accounts;
using CodeBucket.Controllers;
using CodeBucket.Data;
using CodeFramework.Elements;
using CodeFramework.Views;
using CodeFramework.Controllers;
using CodeFramework.Cells;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeBucket
{
	
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
        public override UIWindow Window { get; set; }

        public CodeBucket.Controllers.SlideoutNavigationController Slideout { get; set; }

		// This is the main entry point of the application.
		static void Main(string[] args)
		{
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
			//Set the theme
			SetTheme();

			//Create the window
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            //Always start into the Startup controller
            Window.RootViewController = new StartupController();

			//Make what ever window visible.
            Window.MakeKeyAndVisible();

			//Always return true
			return true;
		}


		/// <summary>
		/// Sets the theme of the application.
		/// </summary>
		private void SetTheme()
		{
			//Set the status bar
			UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, false);
			
			//Set the theming
			UINavigationBar.Appearance.SetBackgroundImage(Images.Titlebar.CreateResizableImage(new UIEdgeInsets(0, 0, 1, 0)), UIBarMetrics.Default);
			
			UIBarButtonItem.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(8, 7, 8, 7)), UIControlState.Normal, UIBarMetrics.Default);
			UISegmentedControl.Appearance.SetBackgroundImage(Images.BarButton.CreateResizableImage(new UIEdgeInsets(8, 7, 8, 7)), UIControlState.Normal, UIBarMetrics.Default);
			
			UIBarButtonItem.Appearance.SetBackgroundImage(Images.BarButtonLandscape.CreateResizableImage(new UIEdgeInsets(8, 7, 8, 7)), UIControlState.Normal, UIBarMetrics.LandscapePhone);
			UISegmentedControl.Appearance.SetBackgroundImage(Images.BarButtonLandscape.CreateResizableImage(new UIEdgeInsets(8, 7, 8, 7)), UIControlState.Normal, UIBarMetrics.LandscapePhone);
			
			//BackButton
			UIBarButtonItem.Appearance.SetBackButtonBackgroundImage(Images.BackButton.CreateResizableImage(new UIEdgeInsets(0, 14, 0, 5)), UIControlState.Normal, UIBarMetrics.Default);
			
			UISegmentedControl.Appearance.SetDividerImage(Images.Divider, UIControlState.Normal, UIControlState.Normal, UIBarMetrics.Default);
			
			UIToolbar.Appearance.SetBackgroundImage(Images.Bottombar.CreateResizableImage(new UIEdgeInsets(0, 0, 0, 0)), UIToolbarPosition.Bottom, UIBarMetrics.Default);
			UISearchBar.Appearance.BackgroundImage = Images.Searchbar;
			
			var textAttrs = new UITextAttributes { TextColor = UIColor.White, TextShadowColor = UIColor.DarkGray, TextShadowOffset = new UIOffset(0, -1) };
			UINavigationBar.Appearance.SetTitleTextAttributes(textAttrs);
			UISegmentedControl.Appearance.SetTitleTextAttributes(textAttrs, UIControlState.Normal);
			
			SearchFilterBar.ButtonBackground = Images.BarButton.CreateResizableImage(new UIEdgeInsets(0, 6, 0, 6));
			SearchFilterBar.FilterImage = Images.Filter;
			
			CodeFramework.Images.Views.Background = Images.Background;

			StyledElement.BgColor = UIColor.FromPatternImage(Images.TableCell);
			UserElement.Default = Images.Anonymous;
			NewsFeedElement.DefaultImage = Images.Anonymous;
            TableViewSectionView.BackgroundImage = Images.Searchbar;

            IssueCellView.User = new UIImage(Images.Person.CGImage, 1.3f, UIImageOrientation.Up);
            IssueCellView.Priority = new UIImage(Images.Priority.CGImage, 1.3f, UIImageOrientation.Up);
            IssueCellView.Pencil = new UIImage(Images.Pencil.CGImage, 1.3f, UIImageOrientation.Up);
            IssueCellView.Cog = new UIImage(Images.Cog.CGImage, 1.3f, UIImageOrientation.Up);

            RepositoryCellView.Commit = new UIImage(Images.ScmType.CGImage, 1.3f, UIImageOrientation.Up);
            RepositoryCellView.Heart = new UIImage(Images.Heart.CGImage, 1.3f, UIImageOrientation.Up);
            RepositoryCellView.Fork = new UIImage(Images.Fork.CGImage, 1.3f, UIImageOrientation.Up);
			
			//Resize the back button only on the iPhone
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				UIBarButtonItem.Appearance.SetBackButtonBackgroundImage(Images.BackButtonLandscape.CreateResizableImage(new UIEdgeInsets(0, 14, 0, 6)), UIControlState.Normal, UIBarMetrics.LandscapePhone);
			}
		}

        public override void ReceiveMemoryWarning(UIApplication application)
        {
            //Remove everything from the cache
            Application.Cache.DeleteAll();

            //Pop back to the root view...
            if (Slideout != null && Slideout.TopView != null && Slideout.TopView.NavigationController != null)
                Slideout.TopView.NavigationController.PopToRootViewController(false);
        }
	}
}

