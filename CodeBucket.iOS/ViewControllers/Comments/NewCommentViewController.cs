using System;
using CoreGraphics;
using Foundation;
using UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Comments;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using System.Reactive;

namespace CodeBucket.ViewControllers.Comments
{
    public class NewCommentViewController : BaseViewController
    {
        private readonly ISubject<Unit> _dismissSubject = new Subject<Unit>();
        private readonly UITextView _textView;

        public NewCommentViewModel ViewModel { get; }

        public IObservable<Unit> Dismissed => _dismissSubject.AsObservable();

        public NewCommentViewController(Func<string, Task> saveAction)
        {
            ViewModel = new NewCommentViewModel(saveAction);

            Title = "New Comment";
            EdgesForExtendedLayout = UIRectEdge.None;

            var discardButton = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);

            NavigationItem.RightBarButtonItem = doneButton;
            NavigationItem.LeftBarButtonItem = discardButton;

            _textView = new UITextView();
            _textView.Font = UIFont.PreferredBody;
            _textView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            // Work around an Apple bug in the UITextView that crashes
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
                _textView.AutocorrectionType = UITextAutocorrectionType.No;

            OnActivation(d =>
            {
                discardButton
                    .GetClickedObservable()
                    .SelectUnit()
                    .BindCommand(ViewModel.DiscardCommand)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.DoneCommand.CanExecute)
                    .Switch()
                    .Subscribe(x => doneButton.Enabled = x)
                    .AddTo(d);

                doneButton
                    .GetClickedObservable()
                    .Do(_ => _textView.ResignFirstResponder())
                    .SelectUnit()
                    .BindCommand(ViewModel.DoneCommand)
                    .AddTo(d);

                _textView
                    .GetChangedObservable()
                    .Subscribe(x => ViewModel.Text = x)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Text)
                    .Subscribe(x => _textView.Text = x)
                    .AddTo(d);

                this.WhenAnyObservable(x => x.ViewModel.DismissCommand)
                    .Select(_ => Unit.Default)
                    .Subscribe(_dismissSubject)
                    .AddTo(d);
            });
        }

        public static NewCommentViewController Present(UIViewController parent, Func<string, Task> saveAction)
        {
            var vc = new NewCommentViewController(saveAction);
            var nav = new ThemedNavigationController(vc);
            parent.PresentViewController(nav, true, null);
            vc.Dismissed.Subscribe(_ => vc.DismissViewController(true, null));
            return vc;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _textView.Frame = ComputeComposerSize(CGRect.Empty);
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