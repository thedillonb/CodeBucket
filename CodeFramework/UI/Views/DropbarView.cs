using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;

namespace CodeFramework.UI.Views
{
    public class DropbarView : UIView
    {
        public static UIImage Image;

        public DropbarView(float width)
            : base (new RectangleF(0, 0, width, Image.Size.Height))
        {
            BackgroundColor = UIColor.FromPatternImage(Image);
            Layer.MasksToBounds = false;
            Layer.ShadowColor = UIColor.Black.CGColor;
            Layer.ShadowOpacity = 0.3f;
            Layer.ShadowOffset = new SizeF(0, 5f);
        }
    }
}

