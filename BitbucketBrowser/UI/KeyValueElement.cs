using System;
using MonoTouch.Dialog;
using System.Drawing;
using MonoTouch.UIKit;


namespace BitbucketBrowser.UI
{
	public class KeyValueElement : OwnerDrawnElement
	{
		private readonly string _key;
		private readonly string _value;
		
		private static UIFont KeyFont = UIFont.BoldSystemFontOfSize(12);
		private static UIFont ValueFont = UIFont.SystemFontOfSize(18);
		
		public KeyValueElement(string key, string value)
			: base(MonoTouch.UIKit.UITableViewCellStyle.Default, "keyvalueelement")
		{
			_key = key;
			_value = value;
		}

		public override void Draw(System.Drawing.RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, MonoTouch.UIKit.UIView view)
		{
			UIColor.White.SetFill();
			context.FillRect(bounds);
			
			UIColor.FromRGB(0.3f, 0.3f, 0.3f).SetColor();
			view.DrawString(
				_key,
				new RectangleF(75f - _key.MonoStringLength(KeyFont), bounds.Height / 2 - KeyFont.LineHeight / 2, 75f, KeyFont.LineHeight),
				KeyFont, 
				UILineBreakMode.TailTruncation
			); 
			
			UIColor.Black.SetColor();
			view.DrawString(
				_value,
				new RectangleF(80f, bounds.Height / 2 - ValueFont.LineHeight / 2, bounds.Width - 80f, ValueFont.LineHeight),
				ValueFont, 
				UILineBreakMode.TailTruncation
			); 
		}

		public override float Height(System.Drawing.RectangleF bounds)
		{
			return 44f;
		}

	}
}

