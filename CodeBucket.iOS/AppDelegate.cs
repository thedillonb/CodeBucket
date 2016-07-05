using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using CodeBucket.Services;
using Foundation;
using ReactiveUI;
using Security;
using UIKit;

namespace CodeBucket
{
    /// <summary>
    /// The UIApplicationDelegate for the application. This class is responsible for launching the 
    /// User Interface of the application, as well as listening (and optionally responding) to 
    /// application events from iOS.
    /// </summary>
    [Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
        public override UIWindow Window { get; set; }

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
            // Stamp the date this was installed (first run)
            StampInstallDate("CodeBucket");

            //Register all services
            ServiceRegistration.Register();

            var exceptionSubject = new Subject<Exception>();
            RxApp.DefaultExceptionHandler = exceptionSubject;
            exceptionSubject.Subscribe(x => AlertDialogService.ShowAlert("Error", x.Message));
            
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

			// Setup theme
			Theme.Setup();

            GoToStartupView();

            Window.MakeKeyAndVisible();
			return true;
		}

        class CustomHttpMessageHandler : DelegatingHandler
        {
            public CustomHttpMessageHandler()
                : base(new HttpClientHandler())
            {
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
            {
                if (!string.Equals(request.Method.ToString(), "get", StringComparison.OrdinalIgnoreCase))
                    NSUrlCache.SharedCache.RemoveAllCachedResponses();
                return base.SendAsync(request, cancellationToken);
            }
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


        private void GoToStartupView()
        {
            var startup = new CodeBucket.ViewControllers.StartupViewController();
            TransitionToViewController(startup);
            MessageBus.Current.Listen<LogoutMessage>().Subscribe(_ => startup.DismissViewController(true, null));
        }

        private void TransitionToViewController(UIViewController viewController)
        {
            UIView.Transition(Window, 0.35, UIViewAnimationOptions.TransitionCrossDissolve, () => 
                Window.RootViewController = viewController, null);
        }

        /// <summary>
        /// Record the date this application was installed (or the date that we started recording installation date).
        /// </summary>
        private static DateTime StampInstallDate(string name)
        {
            try
            {
                var query = new SecRecord(SecKind.GenericPassword) { Service = name, Account = "application" };

                SecStatusCode secStatusCode;
                var queriedRecord = SecKeyChain.QueryAsRecord(query, out secStatusCode);
                if (secStatusCode != SecStatusCode.Success)
                {
                    queriedRecord = new SecRecord(SecKind.GenericPassword)
                    {
                        Label = name + " Install Date",
                        Service = name,
                        Account = query.Account,
                        Description = string.Format("The first date {0} was installed", name),
                        Generic = NSData.FromString(DateTime.UtcNow.ToString())
                    };

                    var err = SecKeyChain.Add(queriedRecord);
                    if (err != SecStatusCode.Success)
                        System.Diagnostics.Debug.WriteLine("Unable to save stamp date!");
                }
                else
                {
                    DateTime time;
                    if (!DateTime.TryParse(queriedRecord.Generic.ToString(), out time))
                        SecKeyChain.Remove(query);
                }

                return DateTime.Parse(NSString.FromData(queriedRecord.Generic, NSStringEncoding.UTF8));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                return DateTime.Now;
            }
        }
	}
}