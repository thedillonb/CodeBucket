using MonoTouch.UIKit;
using System.Drawing;

namespace CodeFramework.UI.Views
{
    public class DropbarView : UIView
    {
        public static UIImage Image;
        private UIView _img;

        public DropbarView(float width)
            : base (new RectangleF(0, 0, width, 0f))
        {
            BackgroundColor = UIColor.FromPatternImage(Image);
            this.ClipsToBounds = false;

            _img = new UIView();
            _img.BackgroundColor = UIColor.FromPatternImage(Image);
            _img.Layer.MasksToBounds = false;
            _img.Layer.ShadowColor = UIColor.Black.CGColor;
            _img.Layer.ShadowOpacity = 0.3f;
            _img.Layer.ShadowOffset = new SizeF(0, 5f);
            AddSubview(_img);

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _img.Frame = new RectangleF(0, 0, Bounds.Width, Image.Size.Height);
        }

    }
}

