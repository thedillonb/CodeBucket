// Composer.cs:
//    Views and ViewControllers for composing messages
//
// Copyright 2010 Miguel de Icaza
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CodeBucket.Controllers
{
	
	/// <summary>
	///   Composer is a singleton that is shared through the lifetime of the app,
	///   the public methods in this class reset the values of the composer on 
	///   each invocation.
	/// </summary>
	public class Composer : UIViewController
	{
	    readonly ComposerView _composerView;
	    readonly UINavigationBar _navigationBar;
	    readonly UINavigationItem _navItem;
		internal UIBarButtonItem SendItem;
		UIViewController _previousController;
        public Action ReturnAction;

        public bool EnableSendButton
        {
            get { return SendItem.Enabled; }
            set { SendItem.Enabled = value; }
        }

        private class ComposerView : UIView 
        {
            const UIBarButtonItemStyle Style = UIBarButtonItemStyle.Bordered;
            internal readonly UITextView textView;
            
            public ComposerView (RectangleF bounds, Composer composer) : base (bounds)
            {
                textView = new UITextView (RectangleF.Empty) {
                    Font = UIFont.SystemFontOfSize (18),
                };
                
                // Work around an Apple bug in the UITextView that crashes
                if (MonoTouch.ObjCRuntime.Runtime.Arch == MonoTouch.ObjCRuntime.Arch.SIMULATOR)
                    textView.AutocorrectionType = UITextAutocorrectionType.No;

                AddSubview (textView);
            }

            
            internal void Reset (string text)
            {
                textView.Text = text;
            }
            
            public override void LayoutSubviews ()
            {
                Resize (Bounds);
            }
            
            void Resize (RectangleF bounds)
            {
                textView.Frame = new RectangleF (0, 0, bounds.Width, bounds.Height);
            }
            
            public string Text { 
                get {
                    return textView.Text;
                }
                set {
                    textView.Text = value;
                }
            }
        }
		
		public Composer () : base (null, null)
		{
            Title = "New Comment";

			// Navigation Bar
		    _navigationBar = new UINavigationBar(new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, 44))
		                         {AutoresizingMask = UIViewAutoresizing.FlexibleWidth, AutosizesSubviews = true};
		    _navItem = new UINavigationItem ("");
			var close = new UIBarButtonItem ("Close", UIBarButtonItemStyle.Plain, (s, e) => CloseComposer());
			_navItem.LeftBarButtonItem = close;
			SendItem = new UIBarButtonItem ("Create", UIBarButtonItemStyle.Plain, PostCallback);
			_navItem.RightBarButtonItem = SendItem;

			_navigationBar.PushNavigationItem (_navItem, false);
			
			// Composer
			_composerView = new ComposerView (ComputeComposerSize (RectangleF.Empty), this);
			
			// Add the views
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UIKeyboardWillShowNotification"), KeyboardWillShow);

			View.AddSubview (_composerView);
			View.AddSubview (_navigationBar);
		}

        public string Text
        {
            get { return _composerView.Text; }
            set { _composerView.Text = value; }
        }

        public string ActionButtonText 
        {
            get { return _navItem.RightBarButtonItem.Title; }
            set { _navItem.RightBarButtonItem.Title = value; }
        }

		public void CloseComposer ()
		{
			SendItem.Enabled = true;
			_previousController.DismissModalViewControllerAnimated (true);
        }

		void PostCallback (object sender, EventArgs a)
		{
			SendItem.Enabled = false;

            if (ReturnAction != null)
                ReturnAction();
		}
		
		void KeyboardWillShow (NSNotification notification)
		{
		    var nsValue = notification.UserInfo.ObjectForKey (UIKeyboard.BoundsUserInfoKey) as NSValue;
		    if (nsValue == null) return;
		    var kbdBounds = nsValue.RectangleFValue;
		    _composerView.Frame = ComputeComposerSize (kbdBounds);
		}

	    RectangleF ComputeComposerSize (RectangleF kbdBounds)
		{
			var view = View.Bounds;
			var nav = _navigationBar.Bounds;

			return new RectangleF (0, nav.Height, view.Width, view.Height-kbdBounds.Height-nav.Height);
		}
       
        public override void ViewWillLayoutSubviews()
        {
            base.ViewWillLayoutSubviews();
            _navigationBar.Frame = new RectangleF (0, 0, View.Bounds.Width, 44);
        }

        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			_composerView.textView.BecomeFirstResponder ();
		}
		
		public void NewComment (UIViewController parent, Action action)
		{
            _navItem.Title = Title;
            ReturnAction = action;
            _previousController = parent;
            _composerView.textView.BecomeFirstResponder ();
            parent.PresentModalViewController (this, true);
		}
	}
}
