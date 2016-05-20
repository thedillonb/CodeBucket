using System;
using CoreGraphics;
using Foundation;
using UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Comments;

namespace CodeBucket.ViewControllers.Comments
{
    public class NewCommentViewController : BaseViewController<NewCommentViewModel>
    {
        private readonly UITextView _textView;

        public NewCommentViewController()
        {
            Title = "New Comment";
            EdgesForExtendedLayout = UIRectEdge.None;

            var closeButton = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            var doneButton = new UIBarButtonItem { Image = Images.Buttons.Save };

            NavigationItem.RightBarButtonItem = doneButton;
            NavigationItem.LeftBarButtonItem = closeButton;

            _textView = new UITextView(ComputeComposerSize(CGRect.Empty));
            _textView.Font = UIFont.PreferredBody;
            _textView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            // Work around an Apple bug in the UITextView that crashes
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
                _textView.AutocorrectionType = UITextAutocorrectionType.No;

            OnActivation(d =>
            {
                closeButton
                    .GetClickedObservable()
                    .InvokeCommand(ViewModel.DismissCommand)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.DismissCommand)
                    .Select(x => x.CanExecuteObservable)
                    .Switch()
                    .Subscribe(x => doneButton.Enabled = x)
                    .AddTo(d);

                doneButton
                    .GetClickedObservable()
                    .Do(_ => _textView.ResignFirstResponder())
                    .InvokeCommand(ViewModel.DoneCommand)
                    .AddTo(d);

                _textView
                    .GetChangedObservable()
                    .Subscribe(x => ViewModel.Text = x)
                    .AddTo(d);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            View.AddSubview(_textView);
        }

        private float CalculateHeight(UIInterfaceOrientation orientation)
        {
            if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                return 44;

            // If  pad
            if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown)
                return 64;
            return 88f;
        }

        public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillRotate(toInterfaceOrientation, duration);

            if (_textView.InputAccessoryView != null)
            {
                UIView.Animate(duration, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    var frame = _textView.InputAccessoryView.Frame;
                    frame.Height = CalculateHeight(toInterfaceOrientation);
                    _textView.InputAccessoryView.Frame = frame;
                }, null);
            }
        }

        void KeyboardWillShow(NSNotification notification)
        {
            var nsValue = notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey) as NSValue;
            if (nsValue == null) return;
            var kbdBounds = nsValue.RectangleFValue;
            UIView.Animate(1.0f, 0, UIViewAnimationOptions.CurveEaseIn, () => _textView.Frame = ComputeComposerSize(kbdBounds), null);
        }

        void KeyboardWillHide(NSNotification notification)
        {
            _textView.Frame = ComputeComposerSize(new CGRect(0, 0, 0, 0));
        }

        CGRect ComputeComposerSize(CGRect kbdBounds)
        {
            var view = View.Bounds;
            return new CGRect(0, 0, view.Width, view.Height - kbdBounds.Height);
        }

        NSObject _hideNotification, _showNotification;
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            _showNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillShowNotification"), KeyboardWillShow);
            _hideNotification = NSNotificationCenter.DefaultCenter.AddObserver(new NSString("UIKeyboardWillHideNotification"), KeyboardWillHide);
            _textView.BecomeFirstResponder();
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (_hideNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_hideNotification);
            if (_showNotification != null)
                NSNotificationCenter.DefaultCenter.RemoveObserver(_showNotification);
        }
    }
}