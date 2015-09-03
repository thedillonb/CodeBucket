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
            : base(UIButtonType.Custom)
        {
            this.AutosizesSubviews = true;

            _imageView = new UIImageView(new CGRect(new CGPoint(0, 0), this.Frame.Size));
            _imageView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
            _imageView.Layer.MasksToBounds = true;
            _imageView.Layer.CornerRadius = 4.0f;

//            this.Layer.ShadowColor = UIColor.Black.CGColor;
//            this.Layer.ShadowOpacity = 0.3f;
//            this.Layer.ShadowOffset = new SizeF(0, 1);
//            this.Layer.ShadowRadius = 4.0f;

            this.AddSubview(_imageView);
        }
    }
}

