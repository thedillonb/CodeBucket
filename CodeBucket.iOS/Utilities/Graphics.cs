using System;
using CoreGraphics;
using UIKit;
using Foundation;

namespace CodeBucket.Utils
{
	public static class Graphics
    {
        public static UIImage ImageFromFont(UIFont font, char character, UIColor fillColor)
        {
            var s = new NSString("" + character);
            var stringSize = s.StringSize(font);
            var maxSize = (nfloat)Math.Max(stringSize.Height, stringSize.Width);
            var size = new CGSize(maxSize, maxSize);

            UIGraphics.BeginImageContextWithOptions(size, false, 0f);
            fillColor.SetFill();

            var drawPoint = new CGPoint((size.Width / 2f) - (stringSize.Width / 2f),
                (size.Height / 2f) - (stringSize.Height / 2f));
            s.DrawString(drawPoint, font);

            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }
	}
}

