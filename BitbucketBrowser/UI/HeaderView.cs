using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BitbucketBrowser.UI
{
	public abstract class HeaderView : UIView
	{
		protected static float XPad = 14f;
		protected static float YPad = 10f;
		
		public HeaderView(RectangleF rect)
			: base(rect)
		{
			Layer.MasksToBounds = false;
			Layer.ShadowColor = UIColor.DarkGray.CGColor;
			Layer.ShadowOpacity = 1.0f;
			Layer.ShadowOffset = new SizeF(0, 3f);
			BackgroundColor = UIColor.FromRGB(0.94f, 0.94f, 0.94f);
		}
	}
}

