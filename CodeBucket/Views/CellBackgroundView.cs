using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using System.Drawing;

namespace CodeBucket.Views
{
    public class CellBackgroundView : UIView
    {
        static readonly CGGradient BottomGradient;
        static readonly CGGradient TopGradient;

        static CellBackgroundView ()
        {
            using (var rgb = CGColorSpace.CreateDeviceRGB()){
                float [] colorsBottom = {
                    1, 1, 1, .5f,
                    0.91f, 0.91f, 0.91f, .5f
                };
                BottomGradient = new CGGradient (rgb, colorsBottom, null);
                float [] colorsTop = {
                    0.94f, 0.94f, 0.94f, .5f,
                    1, 1, 1, 0.5f
                };
                TopGradient = new CGGradient (rgb, colorsTop, null);
            }
        }

        public override void Draw(RectangleF rect)
        {
            var uiTableViewCell = Superview as UITableViewCell;
            var highlighted = uiTableViewCell != null && uiTableViewCell.Highlighted;

            var context = UIGraphics.GetCurrentContext();
            var bounds = Bounds;
            var midx = bounds.Width/2;
            if (!highlighted){
                UIColor.White.SetColor ();
                context.FillRect (bounds);
                context.DrawLinearGradient (BottomGradient, new PointF (midx, bounds.Height-17), new PointF (midx, bounds.Height), 0);
                context.DrawLinearGradient (TopGradient, new PointF (midx, 1), new PointF (midx, 3), 0);
            }
        }
          
    }
}
