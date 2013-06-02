using MonoTouch.UIKit;
using System.Drawing;

namespace CodeBucket.Views
{
    public class TableViewSectionView : UIView
    {
        public static UIImage BackgroundImage;
        readonly UILabel _lbl;

        static readonly MonoTouch.CoreGraphics.CGGradient BottomGradient;
        static readonly MonoTouch.CoreGraphics.CGGradient TopGradient;
        
        static TableViewSectionView ()
        {
            using (var rgb = MonoTouch.CoreGraphics.CGColorSpace.CreateDeviceRGB()){
                float [] colorsBottom = {
                    1, 1, 1, 0f,
                    0.4f, 0.4f, 0.4f, .6f
                };
                BottomGradient = new MonoTouch.CoreGraphics.CGGradient (rgb, colorsBottom, null);
                float [] colorsTop = {
                    0.8f, 0.8f, 0.8f, .4f,
                    1, 1, 1, 0f
                };
                TopGradient = new MonoTouch.CoreGraphics.CGGradient (rgb, colorsTop, null);
            }
        }

        public TableViewSectionView(string text)
        {
            Frame = new RectangleF(0, 0, 320, 24);
            BackgroundColor = UIColor.FromPatternImage(BackgroundImage).ColorWithAlpha(0.9f);
            
            _lbl = new UILabel();
            _lbl.Text = text;
            _lbl.TextColor = UIColor.White;
            _lbl.Font = UIFont.BoldSystemFontOfSize(14f);
            _lbl.BackgroundColor = UIColor.Clear;
            _lbl.ShadowColor = UIColor.FromRGB(120, 120, 120);
            _lbl.ShadowOffset = new System.Drawing.SizeF(1, 1);
            _lbl.LineBreakMode = UILineBreakMode.TailTruncation;
            AddSubview(_lbl);
        }
        
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _lbl.Frame = new RectangleF(10, 2, Bounds.Width - 20f, Bounds.Height - 4f);
        }

        public override void Draw(RectangleF rect)
        {
            var context = UIGraphics.GetCurrentContext();
            var bounds = Bounds;
            var midx = bounds.Width/2;
            
            base.Draw(rect);
            
            context.DrawLinearGradient (BottomGradient, new PointF (midx, bounds.Height-2), new PointF (midx, bounds.Height), 0);
            context.DrawLinearGradient (TopGradient, new PointF (midx, 0), new PointF (midx, 2), 0);
        }
    }
}

