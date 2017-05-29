using System;
using System.Net.Http;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using CodeBucket.Core.Messages;
using CodeBucket.Core.Services;
using CodeBucket.Services;
using Foundation;
using ReactiveUI;
using Security;
using Splat;
using UIKit;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Reactive.Linq;

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
        private Lazy<IApplicationService> _applicationService =
            new Lazy<IApplicationService>(() => Locator.Current.GetService<IApplicationService>());
            
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
            var stampedDate = StampInstallDate("CodeBucket");

            //Register all services
            Services.ServiceRegistration.Register();

            var culture = new System.Globalization.CultureInfo("en");
            System.Threading.Thread.CurrentThread.CurrentCulture = culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            Data.Migration.Migrate();

            var exceptionSubject = new Subject<Exception>();
            RxApp.DefaultExceptionHandler = exceptionSubject;
            exceptionSubject.Subscribe(x => AlertDialogService.ShowAlert("Error", x.Message));

            var purchaseService = Locator.Current.GetService<IInAppPurchaseService>();
            purchaseService.ThrownExceptions.Subscribe(ex =>
            {
                AlertDialogService.ShowAlert("Error Purchasing", ex.Message);
            });
            
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

			Theme.Setup();

            var featuresService = Locator.Current.GetService<IFeaturesService>();

            if (stampedDate <= new DateTime(2016, 7, 30, 0, 0, 0))
            {
                featuresService.ActivateProDirect();
            }

#if DEBUG
            featuresService.ActivateProDirect();
#endif

            //var defaultValueService = Locator.Current.GetService<IDefaultValueService>();

            //bool hasSeenWelcome;
            //if (!defaultValueService.TryGet("HAS_SEEN_WELCOME_INTRO", out hasSeenWelcome) || !hasSeenWelcome)
            //{
            //defaultValueService.Set("HAS_SEEN_WELCOME_INTRO", true);
            //var welcomeViewController = new CodeBucket.ViewControllers.Walkthrough.WelcomePageViewController();
            //welcomeViewController.WantsToDimiss += GoToStartupView;
            //TransitionToViewController(welcomeViewController);
            //}
            //else
            //{
            GoToStartupView();
            //}


            Window.MakeKeyAndVisible();
			return true;
		}

        public override void WillEnterForeground(UIApplication application)
        {
            _applicationService.Value.RefreshToken().ToBackground();
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

            if (string.Equals(url.Host, "login", StringComparison.OrdinalIgnoreCase))
            {
                var queries = url
                    .Query.Split('&')
                    .Select(x => x.Split('='))
                    .ToDictionary(x => x.FirstOrDefault(), x => x.LastOrDefault(), StringComparer.OrdinalIgnoreCase);

                var showError = new Action<string>(msg => AlertDialogService.ShowAlert(
                    "Error",
                    $"There was a problem attempting to login: {msg}."));

                if (queries.TryGetValue("code", out string code))
                {
                    _applicationService
                        .Value.Login(code).ToObservable()
                        .Select(_ => new LogoutMessage())
                        .Subscribe(x => MessageBus.Current.SendMessage(x),
                                   err => showError(err.Message));
                }
                else if (queries.TryGetValue("error", out string error))
                {
                    var errorDescription = "unknown error";
                    if (error == "access_denied")
                        errorDescription = "access denied";

                    showError(errorDescription);
                }
            }

			return true;
		}

        private void GoToStartupView()
        {
            var startup = new ViewControllers.StartupViewController();
            TransitionToViewController(startup);
            MessageBus.Current.Listen<LogoutMessage>()
                      .ObserveOn(RxApp.MainThreadScheduler)
                      .Subscribe(_ => startup.DismissViewController(true, null));
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