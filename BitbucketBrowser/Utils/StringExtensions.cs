using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;


namespace BitbucketBrowser
{
	public static class StringExtensions
	{
		public static float MonoStringLength(this string s, UIFont font)
		{
			if (string.IsNullOrEmpty(s))
				return 0f;
				
			using (NSString str = new NSString (s))
			{
				return str.StringSize(font).Width;
			}
		}
		
		public static float MonoStringHeight(this string s, UIFont font, float maxWidth)
		{
			if (string.IsNullOrEmpty(s))
				return 0f;
			
			using (NSString str = new NSString (s))
			{
				return str.StringSize(font, new SizeF(maxWidth, 1000), UILineBreakMode.WordWrap).Height;
			}
		}
	}
}

