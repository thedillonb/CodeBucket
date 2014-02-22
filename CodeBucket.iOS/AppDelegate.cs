// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the AppDelegate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CodeFramework.iOS;

namespace CodeBucket.iOS
{
	using Cirrious.CrossCore;
	using Cirrious.MvvmCross.Touch.Platform;
	using Cirrious.MvvmCross.ViewModels;
	using MonoTouch.Foundation;
	using MonoTouch.UIKit;

	/// <summary>
	/// The UIApplicationDelegate for the application. This class is responsible for launching the 
	/// User Interface of the application, as well as listening (and optionally responding) to 
	/// application events from iOS.
	/// </summary>
	[Register("AppDelegate")]
	public class AppDelegate : MvxApplicationDelegate
	{
		/// <summary>
		/// The window.
		/// </summary>
		private UIWindow window;

		/// <summary>
		/// This is the main entry point of the application.
		/// </summary>
		/// <param name="args">The args.</param>
		public static void Main(string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}

		/// <summary>
		/// Finished the launching.
		/// </summary>
		/// <param name="app">The app.</param>
		/// <param name="options">The options.</param>
		/// <returns>True or false.</returns>
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
            this.window = new UIWindow(UIScreen.MainScreen.Bounds);
            var presenter = new TouchViewPresenter(this.window);
            var setup = new Setup(this, presenter);
            setup.Initialize();

            Mvx.Resolve<CodeFramework.Core.Services.IAnalyticsService>().Init("UA-42879084-1", "CodeBucket");

			var iRate = MTiRate.iRate.SharedInstance;
            iRate.AppStoreID = 551531422;

			// Setup theme
			Theme.Setup();

			var startup = Mvx.Resolve<IMvxAppStart>();
			startup.Start();

			this.window.MakeKeyAndVisible();

			return true;
		}

		public override bool HandleOpenURL(UIApplication application, NSUrl url)
		{
			if (url == null)
				return false;
			var uri = new System.Uri(url.ToString());

//			if (Slideout != null)
//			{
//				if (!string.IsNullOrEmpty(uri.Host))
//				{
//					string username = uri.Host;
//					string repo = null;
//
//					if (uri.Segments.Length > 1)
//						repo = uri.Segments[1].Replace("/", "");
//
//					if (repo == null)
//						Slideout.SelectView(new CodeBucket.ViewControllers.ProfileViewController(username));
//					else
//						Slideout.SelectView(new CodeBucket.ViewControllers.RepositoryInfoViewController(username, repo, repo));
//				}
//			}

			return true;
		}
	}
}