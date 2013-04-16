using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;
namespace RedPlum
{
	public enum MBProgressHUDMode
	{
		/** Progress is shown using an UIActivityIndicatorView. This is the default. */
		Indeterminate,
		/** Progress is shown using a MBRoundProgressView. */
		Determinate
	}

	public class MBProgressHUD : UIView
	{
//		private UIView _indicator;
//		private UIView Indicator {
//			get {
//				if (_indicator == null) {
//					if (Mode == MBProgressHUDMode.Determinate) {
//						_indicator = new MBRoundProgressView ();
//					} else {
//						_indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
//						((UIActivityIndicatorView)_indicator).StartAnimating ();
//					}
//					this.AddSubview(_indicator);
//				}
//				return _indicator;
//			}
//			set { _indicator = value; }
//		}
		private UIView Indicator;

		private float Width { get; set; }
		private float Height { get; set; }
		private NSTimer GraceTimer { get; set; }
		private NSTimer MinShowTimer { get; set; }
		private DateTime? ShowStarted { get; set; }
		private MBProgressHUDMode? _mode;
		public MBProgressHUDMode Mode {
			get {
				if (!_mode.HasValue) {
					_mode = MBProgressHUDMode.Indeterminate;
					EnsureInvokedOnMainThread (() =>
					{
						UpdateIndicators ();
						SetNeedsLayout ();
						SetNeedsDisplay ();
					});
				}
				return _mode.Value;
			}
			set {
				// Dont change mode if it wasn't actually changed to prevent flickering
				if (_mode == value) {
					return;
				}
				_mode = value;
				EnsureInvokedOnMainThread (() =>
				{
					UpdateIndicators ();
					SetNeedsLayout ();
					SetNeedsDisplay ();
				});
			}
		}
		private NSAction MethodForExecution { get; set; }
		private bool UseAnimation { get; set; }
		private float YOffset { get; set; }
		private float XOffset { get; set; }
		private bool TaskInProgress { get; set; }
		private float GraceTime { get; set; }
		private float MinShowTime { get; set; }
		private UILabel Label { get; set; }
		private UILabel DetailsLabel { get; set; }
		private float _progress;
		public float Progress {
			get { return _progress; }
			set {
				if (_progress != value)
					_progress = value;
				if (Mode == MBProgressHUDMode.Determinate) {
					EnsureInvokedOnMainThread (() =>
					{
						UpdateProgress ();
						SetNeedsDisplay ();
					});
				}
			}
		}
		public event EventHandler HudWasHidden;
		private string _titleText = "Loading";
		public string TitleText {
			get { return _titleText; }
			set {
				if (_titleText != value) {
					_titleText = value;
					EnsureInvokedOnMainThread (() =>
					{
						Label.Text = _titleText;
						SetNeedsLayout ();
						SetNeedsDisplay ();
					});
				}
			}
		}
		private string _detailText;
		public string DetailText {

			get { return _detailText; }
			set {
				if (_detailText != value) {
					_detailText = value;
					EnsureInvokedOnMainThread (() =>
					{
						DetailsLabel.Text = _detailText;
						SetNeedsLayout ();
						SetNeedsDisplay ();
					});
				}
			}
		}
		public float Opacity { get; set; }
		public UIFont TitleFont { get; set; }
		public UIFont DetailFont { get; set; }
		private bool IsFinished { get; set; }


		#region Accessor helpers

		private void UpdateProgress ()
		{
			var indicator = Indicator as MBRoundProgressView;
			if (indicator != null) {
				indicator.Progress = Progress;
			}
		}

		private void UpdateIndicators ()
		{
			if (Indicator != null) {
				Indicator.RemoveFromSuperview ();
			}
			
			this.Indicator = null;
			
			if (Mode == MBProgressHUDMode.Determinate) {
				Indicator = new MBRoundProgressView ();
			} else {
				Indicator = new UIActivityIndicatorView (UIActivityIndicatorViewStyle.WhiteLarge);
				((UIActivityIndicatorView)Indicator).StartAnimating ();
			}
			
			this.AddSubview (Indicator);
		}

		#endregion
		#region Constants

		public const float MARGIN = 20.0f;
		public const float PADDING = 4.0f;

		public const float LABELFONTSIZE = 22.0f;
		public const float LABELDETAILSFONTSIZE = 16.0f;
		#endregion

		#region Lifecycle methods

		public MBProgressHUD (UIWindow window) : base(window.Bounds)
		{
			Initialize ();
		}

		public MBProgressHUD (UIView view) : base(view.Bounds)
		{
			Initialize ();
		}

		public MBProgressHUD (RectangleF frame) : base(frame)
		{
			Initialize ();
		}

		void Initialize ()
		{
			this.Mode = MBProgressHUDMode.Indeterminate;
			
			// Add label
			Label = new UILabel (this.Bounds);
			
			// Add details label
			DetailsLabel = new UILabel (this.Bounds);
			
			this.TitleText = null;
			this.DetailText = null;
			this.Opacity = 0.7f;
			this.TitleFont = UIFont.BoldSystemFontOfSize (LABELFONTSIZE);
			this.DetailFont = UIFont.BoldSystemFontOfSize (LABELDETAILSFONTSIZE);
			this.XOffset = 0.0f;
			this.YOffset = 0.0f;
			this.GraceTime = 0.0f;
			this.MinShowTime = 0.0f;
			
			this.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin;
			
			// Transparent background
			this.Opaque = false;
			this.BackgroundColor = UIColor.Clear;
			
			// Make invisible for now
			this.Alpha = 0.0f;
			
			TaskInProgress = false;
		}
		
		private void EnsureInvokedOnMainThread (Action action)
		{
			if (IsMainThread ())
			{
				action ();
				return;
			}
			this.BeginInvokeOnMainThread (() => action());
		}

		public void Dispose ()
		{
			this.Indicator = null;
			Label.Dispose ();
			Label = null;
			DetailsLabel.Dispose ();
			DetailsLabel = null;
			GraceTimer.Dispose ();
			GraceTimer = null;
			MinShowTimer.Dispose ();
			MinShowTimer = null;
			base.Dispose ();
		}

		#endregion
		#region Layout

		public override void LayoutSubviews ()
		{
			RectangleF frame = this.Bounds;
			
			// Compute HUD dimensions based on indicator size (add margin to HUD border)
			RectangleF indFrame = Indicator.Bounds;
			this.Width = indFrame.Size.Width + 2 * MARGIN;
			this.Height = indFrame.Size.Height + 2 * MARGIN;
			
			// Position the indicator
			indFrame = new RectangleF ((float)Math.Floor ((frame.Size.Width - indFrame.Size.Width) / 2) + this.XOffset, (float)Math.Floor ((frame.Size.Height - indFrame.Size.Height) / 2) + this.YOffset, indFrame.Size.Width, indFrame.Size.Height);
			Indicator.Frame = indFrame;
			
			// Add label if label text was set 
			if (null != this.TitleText) {
				// Get size of label text
				SizeF dims = StringSize (TitleText, this.TitleFont);
				
				// Compute label dimensions based on font metrics if size is larger than max then clip the label width
				float lHeight = dims.Height;
				float lWidth;
				if (dims.Width <= (frame.Size.Width - 2 * MARGIN)) {
					lWidth = dims.Width;
				} else {
					lWidth = frame.Size.Width - 4 * MARGIN;
				}
				
				// Set label properties
				Label.Font = this.TitleFont;
				Label.AdjustsFontSizeToFitWidth = false;
				Label.TextAlignment = UITextAlignment.Center;
				Label.Opaque = false;
				Label.BackgroundColor = UIColor.Clear;
				Label.TextColor = UIColor.White;
				Label.Text = this.TitleText;
				
				// Update HUD size
				if (this.Width < (lWidth + 2 * MARGIN)) {
					this.Width = lWidth + 2 * MARGIN;
				}
				this.Height = this.Height + lHeight + PADDING;
				
				// Move indicator to make room for the label
				indFrame = new RectangleF (indFrame.Location.X, indFrame.Location.Y - (float)(Math.Floor (lHeight / 2 + PADDING / 2)), indFrame.Width, indFrame.Height);
				Indicator.Frame = indFrame;
				
				// Set the label position and dimensions
				RectangleF lFrame = new RectangleF ((float)Math.Floor ((frame.Size.Width - lWidth) / 2) + XOffset, (float)Math.Floor (indFrame.Location.Y + indFrame.Size.Height + PADDING), lWidth, lHeight);
				Label.Frame = lFrame;
				
				this.AddSubview (Label);
				
				// Add details label delatils text was set
				if (null != this.DetailText) {
					// Get size of label text
					dims = StringSize (DetailText, this.DetailFont);
					
					// Compute label dimensions based on font metrics if size is larger than max then clip the label width
					lHeight = dims.Height;
					if (dims.Width <= (frame.Size.Width - 2 * MARGIN)) {
						lWidth = dims.Width;
					} else {
						lWidth = frame.Size.Width - 4 * MARGIN;
					}
					
					// Set label properties
					DetailsLabel.Font = this.DetailFont;
					DetailsLabel.AdjustsFontSizeToFitWidth = false;
					DetailsLabel.TextAlignment = UITextAlignment.Center;
					DetailsLabel.Opaque = false;
					DetailsLabel.BackgroundColor = UIColor.Clear;
					DetailsLabel.TextColor = UIColor.White;
					DetailsLabel.Text = this.DetailText;
					
					// Update HUD size
					if (this.Width < lWidth) {
						this.Width = lWidth + 2 * MARGIN;
					}
					this.Height = this.Height + lHeight + PADDING;
					
					// Move indicator to make room for the new label
					indFrame = new RectangleF (indFrame.Location.X, indFrame.Location.Y - ((float)Math.Floor (lHeight / 2 + PADDING / 2)), indFrame.Width, indFrame.Height);
					Indicator.Frame = indFrame;
					
					// Move first label to make room for the new label
					lFrame = new RectangleF (lFrame.Location.X, lFrame.Location.Y - ((float)Math.Floor (lHeight / 2 + PADDING / 2)), lFrame.Width, lFrame.Height);
					Label.Frame = lFrame;
					
					// Set label position and dimensions
					RectangleF lFrameD = new RectangleF ((float)Math.Floor ((frame.Size.Width - lWidth) / 2) + XOffset, lFrame.Location.Y + lFrame.Size.Height + PADDING, lWidth, lHeight);
					DetailsLabel.Frame = lFrameD;
					
					this.AddSubview (DetailsLabel);
				}
			}

            this.SetNeedsDisplay();
		}

		private static bool IsMainThread() {
            return NSThread.Current.IsMainThread;
		}
		
		#endregion

		#region Showing and execution

		public void Show (bool animated)
		{
			UseAnimation = animated;
			
			// If the grace time is set postpone the HUD display
			if (this.GraceTime > 0.0) {
				this.GraceTimer = NSTimer.CreateScheduledTimer (this.GraceTime, HandleGraceTimer);
				// ... otherwise show the HUD imediately 
			} else {
				this.SetNeedsDisplay ();
				ShowUsingAnimation (UseAnimation);
			}
		}

		public void Hide (bool animated)
		{
			UseAnimation = animated;
			
			// If the minShow time is set, calculate how long the hud was shown,
			// and pospone the hiding operation if necessary
			if (this.MinShowTime > 0.0 && ShowStarted.HasValue) {
				double interv = (DateTime.Now - ShowStarted.Value).TotalSeconds;
				if (interv < this.MinShowTime) {
					this.MinShowTimer = NSTimer.CreateScheduledTimer ((this.MinShowTime - interv), HandleMinShowTimer);
					return;
				}
			}
			
			// ... otherwise hide the HUD immediately
			HideUsingAnimation (UseAnimation);
		}

		void HandleGraceTimer ()
		{
			// Show the HUD only if the task is still running
			if (TaskInProgress) {
				this.SetNeedsDisplay ();
				ShowUsingAnimation (UseAnimation);
			}
		}

		void HandleMinShowTimer ()
		{
			HideUsingAnimation (UseAnimation);
		}

		public void ShowWhileExecuting (NSAction method, bool animated)
		{
			
			MethodForExecution = method;
			
			// Launch execution in new thread
			TaskInProgress = true;
			//TODO: THIS PROBABLY DOES NOT WORK!
			LaunchExecution ();
			
			// Show HUD view
			this.Show (animated);
		}

		void LaunchExecution ()
		{
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				var th = new System.Threading.Thread (() =>
				{
					MethodForExecution.Invoke ();
					this.BeginInvokeOnMainThread (CleanUp);
				});
				th.Start ();
			}
		}

		void AnimationFinished ()
		{
			this.Done ();
		}

		void Done ()
		{
			IsFinished = true;
			
			// If delegate was set make the callback
			this.Alpha = 0.0f;
			
			if (HudWasHidden != null) {
				HudWasHidden (this, EventArgs.Empty);
			}
		}

		void CleanUp ()
		{
			TaskInProgress = false;
			
			this.Indicator = null;
			
			this.Hide (UseAnimation);
		}

		#endregion
		#region Fade in and Fade out

		void ShowUsingAnimation (bool animated)
		{
			this.ShowStarted = DateTime.Now;
			// Fade in
			if (animated) {
				UIView.BeginAnimations (null);
				UIView.SetAnimationDuration (0.40);
				this.Alpha = 1.0f;
				UIView.CommitAnimations ();
			} else {
				this.Alpha = 1.0f;
			}
		}

		void HideUsingAnimation (bool animated)
		{
			// Fade out
			if (animated) {
				if (animated) {
					UIView.BeginAnimations (null);
					UIView.SetAnimationDuration (0.40);
					this.Alpha = .02f;
					NSTimer.CreateScheduledTimer (.4, AnimationFinished);
					UIView.CommitAnimations ();
					
				} else {
					this.Alpha = 0.0f;
					this.Done ();
				}
			}
		}
		#endregion
		#region BG Drawing

		public override void Draw (RectangleF rect)
		{
			// Center HUD
			RectangleF allRect = this.Bounds;
			// Draw rounded HUD bacgroud rect
			RectangleF boxRect = new RectangleF (((allRect.Size.Width - this.Width) / 2) + this.XOffset, ((allRect.Size.Height - this.Height) / 2) + this.YOffset, this.Width, this.Height);
			CGContext ctxt = UIGraphics.GetCurrentContext ();
			this.FillRoundedRect (boxRect, ctxt);
		}

		void FillRoundedRect (RectangleF rect, CGContext context)
		{
			float radius = 10.0f;
			context.BeginPath ();
			context.SetGrayFillColor (0.0f, this.Opacity);
			context.MoveTo (rect.GetMinX () + radius, rect.GetMinY ());
			context.AddArc (rect.GetMaxX () - radius, rect.GetMinY () + radius, radius, (float)(3 * Math.PI / 2), 0f, false);
			context.AddArc (rect.GetMaxX () - radius, rect.GetMaxY () - radius, radius, 0, (float)(Math.PI / 2), false);
			context.AddArc (rect.GetMinX () + radius, rect.GetMaxY () - radius, radius, (float)(Math.PI / 2), (float)Math.PI, false);
			context.AddArc (rect.GetMinX () + radius, rect.GetMinY () + radius, radius, (float)Math.PI, (float)(3 * Math.PI / 2), false);
			context.ClosePath ();
			context.FillPath ();
		}
		
	}

	#endregion

	public class MBRoundProgressView : UIProgressView
	{
		public MBRoundProgressView () : base(new RectangleF (0.0f, 0.0f, 37.0f, 37.0f))
		{
		}

		public override void Draw (RectangleF rect)
		{
			
			
			RectangleF allRect = this.Bounds;
			RectangleF circleRect = new RectangleF (allRect.Location.X + 2, allRect.Location.Y + 2, allRect.Size.Width - 4, allRect.Size.Height - 4);
			
			CGContext context = UIGraphics.GetCurrentContext ();
			
			// Draw background
			context.SetRGBStrokeColor (1.0f, 1.0f, 1.0f, 1.0f);
			// white
			context.SetRGBFillColor (1.0f, 1.0f, 1.0f, 0.1f);
			// translucent white
			context.SetLineWidth (2.0f);
			context.FillEllipseInRect (circleRect);
			context.StrokeEllipseInRect (circleRect);
			
			// Draw progress
			float x = (allRect.Size.Width / 2);
			float y = (allRect.Size.Height / 2);
			context.SetRGBFillColor (1.0f, 1.0f, 1.0f, 1.0f);
			// white
			context.MoveTo (x, y);
			context.AddArc (x, y, (allRect.Size.Width - 4) / 2, -(float)(Math.PI / 2), (float)(this.Progress * 2 * Math.PI) - (float)(Math.PI / 2), false);
			context.ClosePath ();
			context.FillPath ();
		}
		
	}
}

