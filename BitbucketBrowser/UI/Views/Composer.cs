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
using System.Linq;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using System.IO;
using System.Net;
using MonoTouch.AVFoundation;
using System.Text.RegularExpressions;
using System.Threading;

namespace BitbucketBrowser.UI
{
	
	/// <summary>
	///   Composer is a singleton that is shared through the lifetime of the app,
	///   the public methods in this class reset the values of the composer on 
	///   each invocation.
	/// </summary>
	public class Composer : UIViewController
	{
		ComposerView composerView;
		UINavigationBar navigationBar;
		UINavigationItem navItem;
		internal UIBarButtonItem sendItem;
		UIViewController previousController;
        public Action returnAction;

        private class ComposerView : UIView 
        {
            const UIBarButtonItemStyle style = UIBarButtonItemStyle.Bordered;
            internal UITextView textView;
            
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
			// Navigation Bar
			navigationBar = new UINavigationBar (new RectangleF (0, 0, 320, 44));
			navItem = new UINavigationItem ("");
			var close = new UIBarButtonItem ("Close", UIBarButtonItemStyle.Plain, (s, e) => CloseComposer());
			navItem.LeftBarButtonItem = close;
			sendItem = new UIBarButtonItem ("Create", UIBarButtonItemStyle.Plain, PostCallback);
			navItem.RightBarButtonItem = sendItem;

			navigationBar.PushNavigationItem (navItem, false);
			
			// Composer
			composerView = new ComposerView (ComputeComposerSize (RectangleF.Empty), this);
			
			// Add the views
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("UIKeyboardWillShowNotification"), (n) => KeyboardWillShow(n));

			View.AddSubview (composerView);
			View.AddSubview (navigationBar);
		}

        public string Text
        {
            get { return composerView.Text; }
            set { composerView.Text = value; }
        }

		UIImage Scale (UIImage image, SizeF size)
		{
			UIGraphics.BeginImageContext (size);
			image.Draw (new RectangleF (new PointF (0, 0), size));
			var ret = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			return ret;
		}
		
		public void ReleaseResources ()
		{
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();			
		}

		public void CloseComposer ()
		{
			sendItem.Enabled = true;
			previousController.DismissModalViewControllerAnimated (true);
		}		
		
		void PostCallback (object sender, EventArgs a)
		{
			sendItem.Enabled = false;

            if (returnAction != null)
                returnAction();

            sendItem.Enabled = true;
		}
		
		void KeyboardWillShow (NSNotification notification)
		{
			var kbdBounds = (notification.UserInfo.ObjectForKey (UIKeyboard.BoundsUserInfoKey) as NSValue).RectangleFValue;
			
			composerView.Frame = ComputeComposerSize (kbdBounds);
		}

		RectangleF ComputeComposerSize (RectangleF kbdBounds)
		{
			var view = View.Bounds;
			var nav = navigationBar.Bounds;

			return new RectangleF (0, nav.Height, view.Width, view.Height-kbdBounds.Height-nav.Height);
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			composerView.textView.BecomeFirstResponder ();
		}
		
		public void NewComment (UIViewController parent, Action action)
		{
            navItem.Title = "New Comment";
            returnAction = action;
            previousController = parent;
            composerView.textView.BecomeFirstResponder ();
            parent.PresentModalViewController (this, true);
		}
	}
}
