using System;
using UIKit;
using CoreGraphics;

namespace CodeBucket.ViewControllers.SlideoutNavigation
{
    public class MainNavigationController : UINavigationController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
        /// </summary>
        /// <param name="rootViewController">Root view controller.</param>
        /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
        public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
            : this(rootViewController, slideoutNavigationController,
                   new UIBarButtonItem(Images.Buttons.ThreeLines, UIBarButtonItemStyle.Plain, (s, e) => { }))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MainNavigationController"/> class.
        /// </summary>
        /// <param name="rootViewController">Root view controller.</param>
        /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
        /// <param name="openMenuButton">Open menu button.</param>
        public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController, UIBarButtonItem openMenuButton)
            : base(rootViewController)
        {
            openMenuButton.Clicked += (s, e) => slideoutNavigationController.Open(true);
            rootViewController.NavigationItem.LeftBarButtonItem = openMenuButton;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            this.Delegate = new NavigationControllerDelegate();
            InteractivePopGestureRecognizer.Enabled = true;
        }

        /// <Docs>The view controller to push onto the navigation stack</Docs>
        /// <summary>
        /// Pushes a view controller onto the UINavigationController's navigation stack.
        /// </summary>
        /// <see cref="T:MonoTouch.UIKit.UITabBarController"></see>
        /// <param name="viewController">View controller.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void PushViewController(UIViewController viewController, bool animated)
        {
            // To avoid corruption of the navigation stack during animations disabled the pop gesture
            if (InteractivePopGestureRecognizer != null)
                InteractivePopGestureRecognizer.Enabled = false;
            base.PushViewController(viewController, animated);
        }

        private class NavigationControllerDelegate : UINavigationControllerDelegate
        {
            public override void DidShowViewController(UINavigationController navigationController, UIViewController viewController, bool animated)
            {
                // Enable the gesture after the view has been shown
                navigationController.InteractivePopGestureRecognizer.Enabled = true;
            }
        }
    }

    public class MenuNavigationController : UINavigationController
    {
        private readonly SlideoutNavigationController _slideoutNavigationController;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
        /// </summary>
        /// <param name="rootViewController">Root view controller.</param>
        /// <param name="slideoutNavigationController">Slideout navigation controller.</param>
        public MenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
            : base(rootViewController)
        {
            _slideoutNavigationController = slideoutNavigationController;
        }

        /// <summary>
        /// Pushes the view controller.
        /// </summary>
        /// <param name="viewController">View controller.</param>
        /// <param name="animated">If set to <c>true</c> animated.</param>
        public override void PushViewController(UIViewController viewController, bool animated)
        {
            if (_slideoutNavigationController == null)
            {
                base.PushViewController(viewController, animated);
            }
            else
            {
                _slideoutNavigationController.SetMainViewController(
                    new MainNavigationController(viewController, _slideoutNavigationController), animated);
            }
        }
    }

    public enum SlideHandle
    {
        None,
        NavigationBar,
        Full
    }

    public class SlideoutNavigationController : UIViewController
    {
        private readonly static Action EmptyAction = () => { };

        private UIViewController _mainViewController;
        private UIViewController _menuViewController;
        private UITapGestureRecognizer _tapGesture;
        private UIPanGestureRecognizer _panGesture;
        private nfloat _panTranslationX;
        private nfloat _slideHandleHeight;
        private nfloat _menuWidth;
        private SlideHandle _slideHandle;

        public bool IsOpen { get; private set; }

        public float OpenAnimationDuration { get; set; }

        public float VelocityTrigger { get; set; }

        protected UIView ContainerView { get; private set; }

        public bool PanGestureEnabled { get; set; }

        /// <summary>
        /// Gets or sets the amount of visible space the menu is given when the user opens it.
        /// This number is how many pixles you want the top view to slide away from the left side.
        /// </summary>
        /// <value>The width of the menu open.</value>
        public nfloat MenuWidth
        {
            get { return _menuWidth; }
            set
            {
                _menuWidth = value;
                if (_menuViewController != null)
                {
                    var frame = _menuViewController.View.Frame;
                    frame.Width = value;
                    _menuViewController.View.Frame = frame;
                }
            }
        }

        public SlideHandle SlideHandle
        {
            get { return _slideHandle; }
            set
            {
                _slideHandle = value;
                if (value == SlideHandle.None)
                    _slideHandleHeight = 0;
                else if (value == SlideHandle.NavigationBar)
                    _slideHandleHeight = 44f + 20f;
                else if (value == SlideHandle.Full)
                    _slideHandleHeight = float.MaxValue;
            }
        }

        public bool EnableInteractivePopGestureRecognizer { get; set; }

        protected UIViewAnimationOptions AnimationOption { get; set; }

        protected float SlideHalfwayOffset { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether shadowing is enabled
        /// </summary>
        public bool ShadowEnabled { get; set; }

        public UIViewController MenuViewController
        {
            get { return _menuViewController; }
            set
            {
                if (IsViewLoaded)
                    SetMenuViewController(value, false);
                else
                    _menuViewController = value;
            }
        }

        public UIViewController MainViewController
        {
            get { return _mainViewController; }
            set
            {
                if (IsViewLoaded)
                    SetMainViewController(value, false);
                else
                    _mainViewController = value;
            }
        }

        public SlideoutNavigationController()
        {
            OpenAnimationDuration = 0.3f;
            PanGestureEnabled = true;
            AnimationOption = UIViewAnimationOptions.CurveEaseInOut;
            SlideHandle = SlideHandle.Full;
            EnableInteractivePopGestureRecognizer = true;
            SlideHalfwayOffset = 120f;
            VelocityTrigger = 800f;
            MenuWidth = 290f;
            ShadowEnabled = true;

            ContainerView = new UIView();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            IsOpen = true;

            var containerFrame = View.Bounds;
            containerFrame.X = View.Bounds.Width;
            ContainerView.Frame = containerFrame;
            ContainerView.BackgroundColor = UIColor.White;
            ContainerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;

            View.BackgroundColor = UIColor.White;

            _tapGesture = new UITapGestureRecognizer();
            _tapGesture.AddTarget(() => Close(true));
            _tapGesture.NumberOfTapsRequired = 1;

            _panGesture = new UIPanGestureRecognizer
            {
                Delegate = new PanDelegate(this),
                MaximumNumberOfTouches = 1,
                MinimumNumberOfTouches = 1
            };
            _panGesture.AddTarget(() => Pan(ContainerView));
            ContainerView.AddGestureRecognizer(_panGesture);

            if (_menuViewController != null)
                SetMenuViewController(_menuViewController, false);
            if (_mainViewController != null)
                SetMainViewController(_mainViewController, false);

            //Create some shadowing
            if (ShadowEnabled)
            {
                ContainerView.Layer.ShadowOffset = new CGSize(-5, 0);
                ContainerView.Layer.ShadowPath = UIBezierPath.FromRect(ContainerView.Bounds).CGPath;
                ContainerView.Layer.ShadowRadius = 3.0f;
                ContainerView.Layer.ShadowColor = UIColor.Black.CGColor;
            }
        }

        /// <summary>
        /// Animate the specified menuView and mainView based on a percentage.
        /// </summary>
        /// <param name="menuView">The menu view.</param>
        /// <param name="mainView">The main view.</param>
        /// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
        private void Animate(UIView menuView, UIView mainView, nfloat percentage)
        {
            if (percentage > 1)
                percentage = 1;

            // Determine if shadow should be shown
            if (ShadowEnabled)
            {
                if (percentage <= 0)
                    mainView.Layer.ShadowOpacity = 0;
                else
                    ContainerView.Layer.ShadowOpacity = 0.3f;
            }

            var x = View.Bounds.X + (MenuWidth * percentage);
            mainView.Frame = new CGRect(new CGPoint(x, mainView.Frame.Y), mainView.Frame.Size);
        }

        private void Pan(UIView view)
        {
            if (_panGesture.State == UIGestureRecognizerState.Began)
            {
                if (!IsOpen)
                {
                    if (_menuViewController != null)
                        _menuViewController.ViewWillAppear(true);
                }
            }
            else if (_panGesture.State == UIGestureRecognizerState.Changed)
            {
                _panTranslationX = _panGesture.TranslationInView(View).X;
                var total = MenuWidth;
                var numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
                var percentage = numerator / total;
                if (percentage < 0)
                    percentage = 0;

                Action animation = () => Animate(_menuViewController.View, ContainerView, percentage);
                UIView.Animate(0.01f, 0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.AllowUserInteraction, animation, EmptyAction);
            }
            else if (_panGesture.State == UIGestureRecognizerState.Ended || _panGesture.State == UIGestureRecognizerState.Cancelled)
            {
                var velocity = _panGesture.VelocityInView(View).X;
                var total = MenuWidth;
                var numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
                var percentage = numerator / total;
                var animationTime = (nfloat)Math.Min(1 / (Math.Abs(velocity) / 100), OpenAnimationDuration);

                if (IsOpen)
                {
                    if (percentage > .66f && velocity > -VelocityTrigger)
                    {
                        Action animation = () => Animate(_menuViewController.View, ContainerView, 1);
                        UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
                    }
                    else
                        Close(true, animationTime);
                }
                else
                {
                    if (percentage < .33f && velocity < VelocityTrigger)
                    {
                        Action animation = () => Animate(_menuViewController.View, ContainerView, 0);
                        UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
                    }
                    else
                        Open(true, animationTime);
                }
            }
        }

        public void Open(bool animated)
        {
            Open(animated, OpenAnimationDuration);
        }

        private void Open(bool animated, nfloat animationTime)
        {
            if (IsOpen)
                return;

            if (_menuViewController != null)
                _menuViewController.ViewWillAppear(animated);

            Action animation = () => Animate(_menuViewController.View, ContainerView, 1);
            Action completion = () =>
            {
                IsOpen = true;
                ContainerView.AddGestureRecognizer(_tapGesture);

                if (_menuViewController != null)
                    _menuViewController.ViewDidAppear(animated);
            };

            if (ContainerView.Subviews.Length > 0)
                ContainerView.Subviews[0].UserInteractionEnabled = false;

            if (animated)
            {
                UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
            }
            else
            {
                animation();
                completion();
            }
        }

        public void Close(bool animated)
        {
            Close(animated, OpenAnimationDuration);
        }

        private void Close(bool animated, nfloat animationTime)
        {
            if (!IsOpen)
                return;

            if (_menuViewController != null)
                _menuViewController.ViewWillDisappear(animated);

            Action animation = () => Animate(_menuViewController.View, ContainerView, 0);
            Action completion = () =>
            {
                IsOpen = false;

                if (ContainerView.Subviews.Length > 0)
                    ContainerView.Subviews[0].UserInteractionEnabled = true;
                ContainerView.RemoveGestureRecognizer(_tapGesture);

                if (_menuViewController != null)
                    _menuViewController.ViewDidDisappear(animated);
            };

            if (animated)
            {
                UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
            }
            else
            {
                animation();
                completion();
            }
        }

        public void SetMainViewController(UIViewController viewController, bool animated)
        {

            // This will only happen once...
            if (ContainerView.Superview == null)
            {
                var containerFrame = View.Bounds;
                containerFrame.X = View.Bounds.Width;
                ContainerView.Frame = containerFrame;

                View.AddSubview(ContainerView);
                var updatedMenuFrame = new CGRect(View.Bounds.Location, new CGSize(MenuWidth, View.Bounds.Height));
                UIView.Animate(OpenAnimationDuration, 0, UIViewAnimationOptions.BeginFromCurrentState | AnimationOption,
                    () => MenuViewController.View.Frame = updatedMenuFrame, null);

                if (_menuViewController != null)
                    _menuViewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
            }

            AddChildViewController(viewController);
            viewController.View.Frame = ContainerView.Bounds;
            ContainerView.AddSubview(viewController.View);

            if (_mainViewController != null && viewController != _mainViewController)
            {
                _mainViewController.RemoveFromParentViewController();
                _mainViewController.View.RemoveFromSuperview();
                _mainViewController.DidMoveToParentViewController(null);
            }

            Close(animated);
            _mainViewController = viewController;
        }

        public void SetMenuViewController(UIViewController viewController, bool animated)
        {
            this.AddChildViewController(viewController);

            var resizing = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            var width = View.Bounds.Width;
            if (MainViewController != null)
            {
                width = MenuWidth;
                resizing = UIViewAutoresizing.FlexibleHeight;
            }

            viewController.View.Frame = new CGRect(View.Bounds.Location, new CGSize(width, View.Bounds.Height));
            viewController.View.AutoresizingMask = resizing;
            View.InsertSubview(viewController.View, 0);

            if (_menuViewController != null && viewController != _menuViewController)
            {
                _menuViewController.RemoveFromParentViewController();
                _menuViewController.View.RemoveFromSuperview();
                _menuViewController.DidMoveToParentViewController(null);
            }

            _menuViewController = viewController;
        }

        public override bool ShouldAutorotate()
        {
            return true;
        }

        public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations()
        {
            return UIInterfaceOrientationMask.All;
        }

        ///<summary>
        /// A custom UIGestureRecognizerDelegate activated only when the controller 
        /// is visible or touch is within the 44.0f boundary.
        /// 
        /// Special thanks to Gerry High for this snippet!
        ///</summary>
        private class PanDelegate : UIGestureRecognizerDelegate
        {
            private readonly SlideoutNavigationController _controller;

            public PanDelegate(SlideoutNavigationController controller)
            {
                _controller = controller;
            }

            public override bool ShouldBegin(UIGestureRecognizer recognizer)
            {
                if (!_controller.PanGestureEnabled)
                    return false;

                if (_controller.IsOpen)
                    return true;

                var rec = (UIPanGestureRecognizer)recognizer;
                var velocity = rec.VelocityInView(_controller.ContainerView);
                return velocity.X > 0 && Math.Abs(velocity.X) > Math.Abs(velocity.Y);
            }

            public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
            {
                if (!_controller.PanGestureEnabled)
                    return false;

                if (_controller.IsOpen)
                    return true;

                var locationInView = touch.LocationInView(_controller.ContainerView);

                if (locationInView.Y <= _controller._slideHandleHeight)
                {
                    // This is a hack that I don't like but need for now
                    var topNavigationController = _controller.MainViewController as UINavigationController;
                    if (topNavigationController != null && topNavigationController.ViewControllers.Length == 1)
                        return true;

                    if (_controller.EnableInteractivePopGestureRecognizer && locationInView.X <= 10f)
                        return false;
                    return true;
                }

                return false;
            }
        }
    }
}
