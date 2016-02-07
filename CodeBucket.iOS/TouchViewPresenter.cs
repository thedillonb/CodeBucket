using System;
using MvvmCross.Platform;
using MvvmCross.Core.ViewModels;
using CodeBucket.ViewControllers;
using UIKit;
using MonoTouch.SlideoutNavigation;
using CodeBucket.Views;
using CodeBucket.Core;
using CodeBucket.Views.Accounts;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.iOS.Views;

namespace CodeBucket
{
    public class IosViewPresenter : MvxBaseIosViewPresenter
    {
        private readonly UIWindow _window;
        private UINavigationController _generalNavigationController;
        private SlideoutNavigationController _slideoutController;
        private IMvxModalIosView _currentModal;

        public IosViewPresenter(UIWindow window)
        {
            _window = window;
        }

        public override void ChangePresentation(MvxPresentationHint hint)
        {
            var closeHint = hint as MvxClosePresentationHint;
            if (closeHint != null)
            {
                if (_currentModal != null)
                {
                    ((UIViewController)_currentModal).DismissViewController(true, null);
                    return;
                }

                for (int i = _generalNavigationController.ViewControllers.Length - 1; i >= 1; i--)
                {
                    var vc = _generalNavigationController.ViewControllers[i];
                    var touchView = vc as IMvxIosView;
                    if (touchView != null && touchView.ViewModel == closeHint.ViewModelToClose)
                    {
                        _generalNavigationController.PopToViewController(_generalNavigationController.ViewControllers[i - 1], true);
                        return;
                    }
                }

                //If it didnt trigger above it's because it was probably the root.
                _generalNavigationController.PopToRootViewController(true);
            }
        }

        public override void Show(MvxViewModelRequest request)
        {
            var uiView = this.CreateViewControllerFor(request) as UIViewController;

            if (uiView == null)
                throw new InvalidOperationException("Asking to show a view which is not a UIViewController!");

            if (uiView is IMvxModalIosView)
            {
                if (_currentModal != null)
                    throw new InvalidOperationException("Cannot have multiple modals");

                _currentModal = (IMvxModalIosView)uiView;
                var modalNavigationController = new UINavigationController(uiView);
                modalNavigationController.NavigationBar.Translucent = false;
                modalNavigationController.Toolbar.Translucent = false;
                uiView.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.CancelButton, UIBarButtonItemStyle.Plain, (s, e) =>
                {
                    var vm = ((IMvxModalIosView)uiView).ViewModel;
                    Mvx.Resolve<MvvmCross.Plugins.Messenger.IMvxMessenger>().Publish(new CodeFramework.Core.Messages.CancelationMessage(vm));
                    modalNavigationController.DismissViewController(true, null);
                    _currentModal = null;
                });
                PresentModalViewController(modalNavigationController, true);
            }
            else if (uiView is StartupView)
            {
                Transition(uiView);
            }
            else if (uiView is LoginView && _slideoutController == null)
            {
                var nav = new UINavigationController(uiView);
                _window.RootViewController.PresentViewController(nav, true, null);
            }
            else if (uiView is MenuBaseViewController)
            {
                _slideoutController = new SlideoutNavigationController();
                _slideoutController.MenuViewController = new MenuNavigationController(uiView, _slideoutController);
                uiView.NavigationController.NavigationBar.SetBackgroundImage(null, UIBarMetrics.Default);
                uiView.NavigationController.NavigationBar.Translucent = false;
                uiView.NavigationController.Toolbar.Translucent = false;
                uiView.NavigationController.NavigationBar.BarTintColor = UIColor.FromRGB(50, 50, 50);
                Transition(_slideoutController);
            }
            else
            {
                if (request.PresentationValues != null && request.PresentationValues.ContainsKey(PresentationValues.SlideoutRootPresentation))
                {
                    var mainNavigationController = new MainNavigationController(uiView, _slideoutController, new UIBarButtonItem(Theme.CurrentTheme.ThreeLinesButton, UIBarButtonItemStyle.Plain, (s, e) => _slideoutController.Open(true)));
                    _generalNavigationController = mainNavigationController;
                    _slideoutController.SetMainViewController(mainNavigationController, true);


                    //_generalNavigationController.NavigationBar.BarTintColor = Theme.CurrentTheme.ApplicationNavigationBarTint;
                    _generalNavigationController.NavigationBar.Translucent = false;
                    _generalNavigationController.Toolbar.Translucent = false;
                }
                else
                {
                    _generalNavigationController.PushViewController(uiView, true);
                }
            }
        }

        public override bool PresentModalViewController(UIViewController viewController, bool animated)
        {
            if (_window.RootViewController == null)
                Transition(viewController);
            else
                _window.RootViewController.PresentViewController(viewController, true, null);
            return true;
        }

        private void Transition(UIViewController controller)
        {
            // Quickly swap to get the right sizes for the views
            var current = _window.RootViewController;
            _window.RootViewController = controller;
            _window.RootViewController = current;

            UIView.Transition(_window, 0.3f, 
                UIViewAnimationOptions.TransitionCrossDissolve | UIViewAnimationOptions.AllowAnimatedContent, 
                () => _window.RootViewController = controller, null);
        }
    }
}
