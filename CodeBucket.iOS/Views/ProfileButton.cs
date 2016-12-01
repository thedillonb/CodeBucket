using System;
using UIKit;
using CoreGraphics;
using Foundation;
using SDWebImage;

namespace CodeBucket.Views
{
    public class ProfileButton : UIButton
    {
        private readonly UIImageView _imageView;
        private Uri _uri;

        public Uri Uri
        {
            get {
                return _uri;
            }
            set {
                _uri = value;
                if (value != null)
                    _imageView.SetImage(new NSUrl(value.AbsoluteUri));
                else
                    _imageView.Image = null;
            }
        }

        public ProfileButton()
        {
            AutosizesSubviews = true;

            _imageView = new UIImageView(new CGRect(new CGPoint(0, 0), this.Frame.Size));
            _imageView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _imageView.Layer.MasksToBounds = true;

           AddSubview(_imageView);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _imageView.Layer.CornerRadius = Frame.Width / 2;
        }
    }
}

