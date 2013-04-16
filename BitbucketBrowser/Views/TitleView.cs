using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace CodeFramework.UI.Views
{
    public class TitleView : UIView
    {
        UILabel _caption;
        UILabel _subcaption;

        public void SetCaption(string caption, string subcaption)
        {
            _caption.Text = caption;
            _subcaption.Text = subcaption;

            this.SetNeedsLayout();
            this.SetNeedsDisplay();
        }

        public TitleView()
            : base(new RectangleF(0f, 0f, 100f, 32f))
        {
            BackgroundColor = UIColor.Clear;

            _caption = new UILabel(Frame);
            _caption.ShadowColor = UIColor.FromWhiteAlpha(0f, 0.5f);
            _caption.ShadowOffset = new SizeF(0, -1);
            _caption.TextColor = UIColor.White;
            _caption.Font = UIFont.BoldSystemFontOfSize(20f);
            _caption.BackgroundColor = UIColor.Clear;
            _caption.TextAlignment = UITextAlignment.Center;
            AddSubview(_caption);

            _subcaption = new UILabel(Frame);
            _subcaption.ShadowOffset = new SizeF(0, -1);
            _subcaption.TextColor = UIColor.White;
            _subcaption.ShadowColor = UIColor.FromWhiteAlpha(0f, 0.5f);
            _subcaption.Font = UIFont.BoldSystemFontOfSize(9f);
            _subcaption.BackgroundColor = UIColor.Clear;
            _subcaption.TextAlignment = UITextAlignment.Center;
            AddSubview(_subcaption);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var y = 2;
            if (string.IsNullOrEmpty(_subcaption.Text))
            {
                _caption.Frame = new RectangleF(0, y, Bounds.Width, Bounds.Height - 2);
                _subcaption.Hidden = true;
            }
            else
            {
                _caption.Frame = new RectangleF(0, y, Bounds.Width, 21f);
                _subcaption.Frame = new RectangleF(0, 21f, Bounds.Width, 10f);
                _subcaption.Hidden = false;
            }
        }
    }
}

