using System;
using CoreGraphics;
using Foundation;
using UIKit;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Threading.Tasks;

namespace CodeBucket.ViewControllers
{
    public class ComposerViewController : BaseViewController
    {
        private readonly ISubject<Unit> _dismissObservable = new Subject<Unit>();
        private readonly UITextView TextView;
        private readonly UIBarButtonItem _doneButton;

        public IObservable<Unit> WantsDismiss => _dismissObservable.AsObservable();

        public string Text
        {
            get { return TextView.Text; }
            set 
            {
                if (TextView.Text == value)
                    return;
                
                TextView.Text = value;
                this.RaisePropertyChanged();
            }
        }

        public ComposerViewController(Func<string, Task> doneAction)
        {
            Title = "New Comment";
            EdgesForExtendedLayout = UIRectEdge.None;

            var close = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            NavigationItem.LeftBarButtonItem = close;
            _doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Save);
            NavigationItem.RightBarButtonItem = _doneButton;

            TextView = new UITextView(ComputeComposerSize(CGRect.Empty));
            TextView.Font = UIFont.PreferredBody;
            TextView.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;

            // Work around an Apple bug in the UITextView that crashes
            if (ObjCRuntime.Runtime.Arch == ObjCRuntime.Arch.SIMULATOR)
                TextView.AutocorrectionType = UITextAutocorrectionType.No;

            View.AddSubview(TextView);

            OnActivation(d =>
            {
                close
                    .GetClickedObservable()
                    .Select(_ => Unit.Default)
                    .Subscribe(_dismissObservable)
                    .AddTo(d);

                _doneButton
                    .GetClickedObservable()
                    .Subscribe(async _ =>
                    {
                        TextView.ResignFirstResponder();
                        _doneButton.Enabled = false;
                        try
                        {
                            await doneAction(TextView.Text);
                            _dismissObservable.OnNext(Unit.Default);
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine(e.Message + " - " + e.StackTrace);
                        }
                        finally
                        {
                            _doneButton.Enabled = true;
                        }

                    })
                    .AddTo(d);

                TextView
                    .GetChangedObservable()
                    .Subscribe(x => this.RaisePropertyChanged(nameof(Text)))
                    .AddTo(d);
                        

                this.WhenAnyValue(x => x.Text)
                    .Select(x => x.Length > 0)
                    .Subscribe(x => _doneButton.Enabled = x)
                    .AddTo(d);
            });
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

            if (TextView.InputAccessoryView != null)
            {
                UIView.Animate(duration, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
                    var frame = TextView.InputAccessoryView.Frame;
                    frame.Height = CalculateHeight(toInterfaceOrientation);
                    TextView.InputAccessoryView.Frame = frame;
                }, null);
            }
        }

        void KeyboardWillShow(NSNotification notification)
        {
            var nsValue = notification.UserInfo.ObjectForKey(UIKeyboard.BoundsUserInfoKey) as NSValue;
            if (nsValue == null) return;
            var kbdBounds = nsValue.RectangleFValue;
            UIView.Animate(1.0f, 0, UIViewAnimationOptions.CurveEaseIn, () => TextView.Frame = ComputeComposerSize(kbdBounds), null);
        }

        void KeyboardWillHide(NSNotification notification)
        {
            TextView.Frame = ComputeComposerSize(new CGRect(0, 0, 0, 0));
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
            TextView.BecomeFirstResponder();
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