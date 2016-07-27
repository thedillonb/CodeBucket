using System;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using System.Linq;
using BigTed;
using System.Reactive.Disposables;
using CodeBucket.Core.Services;
using Splat;
using CodeBucket.Services;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Application
{
    public class UpgradeViewController : BasicWebViewController
    {
        private readonly IFeaturesService _featuresService = Locator.Current.GetService<IFeaturesService>();
        private readonly IInAppPurchaseService _inAppPurchaseService = Locator.Current.GetService<IInAppPurchaseService>();
        private UIActivityIndicatorView _activityView;

        public UpgradeViewController() : base(false, false)
        {
            Title = "Pro Upgrade";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _activityView = new UIActivityIndicatorView
            {
                Color = Theme.CurrentTheme.PrimaryColor,
                AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
            };
            _activityView.Frame = new CoreGraphics.CGRect(0, 44, View.Frame.Width, 88f);

            Load().ToBackground();
        }

        private async Task Load()
        {
            Web.UserInteractionEnabled = false;
            Web.LoadHtmlString("", NSBundle.MainBundle.BundleUrl);

            _activityView.Alpha = 1;
            _activityView.StartAnimating();
            View.Add(_activityView);

            try
            {
                var request = _inAppPurchaseService.RequestProductData(FeaturesService.ProEdition).WithTimeout(TimeSpan.FromSeconds(30));
                var productData = (await request).Products.FirstOrDefault();
                var enabled = _featuresService.IsProEnabled;
                var model = new UpgradeDetailsModel(productData != null ? productData.LocalizedPrice() : null, enabled);
                var content = new UpgradeDetailsView { Model = model }.GenerateString();
                LoadContent(content);
                Web.UserInteractionEnabled = true;
            }
            catch (Exception e)
            {
                AlertDialogService.ShowAlert("Error Loading Upgrades", e.Message);
            }
            finally
            {
                UIView.Animate(0.2f, 0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.CurveEaseInOut,
                    () => _activityView.Alpha = 0, () =>
                    {
                        _activityView.RemoveFromSuperview();
                        _activityView.StopAnimating();
                    });
            }
        }

        protected override bool ShouldStartLoad(WebKit.WKWebView webView, WebKit.WKNavigationAction navigationAction)
        {
            var url = navigationAction.Request.Url;

            if (url.Scheme.Equals("app"))
            {
                var func = url.Host;

                if (string.Equals(func, "buy", StringComparison.OrdinalIgnoreCase))
                {
                    Activate(_featuresService.ActivatePro).ToBackground();
                }
                else if (string.Equals(func, "restore", StringComparison.OrdinalIgnoreCase))
                {
                    Activate(_featuresService.RestorePro).ToBackground();
                }

                return false;
            }

            if (url.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase))
            {
                UIApplication.SharedApplication.OpenUrl(url);
                return false;
            }

            if (url.Scheme.Equals("file"))
            {
                return true;
            }

            if (url.Scheme.Equals("http") || url.Scheme.Equals("https"))
            {
                var view = new WebBrowserViewController(url.AbsoluteString);
                PresentViewController(view, true, null);
                return false;
            }

            return false;
        }

        private async Task Activate(Func<Task> activation)
        {
            try
            {
                BTProgressHUD.ShowContinuousProgress("Activating...", ProgressHUD.MaskType.Gradient);
                using (Disposable.Create(BTProgressHUD.Dismiss))
                    await activation();
                Load().ToBackground();
            }
            catch (Exception e)
            {
                AlertDialogService.ShowAlert("Error", e.Message);
            }
        }
    }

    public static class UpgradeViewControllerExtensions
    {
        public static UIViewController PresentUpgradeViewController(this UIViewController @this)
        {
            var vc = new UpgradeViewController();
            var nav = new ThemedNavigationController(vc);

            var navObj = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            vc.OnActivation(disposble => navObj.GetClickedObservable().Subscribe(_ => @this.DismissViewController(true, null)).AddTo(disposble));
            vc.NavigationItem.LeftBarButtonItem = navObj;
            @this.PresentViewController(nav, true, null);
            return vc;
        }
    }
}
