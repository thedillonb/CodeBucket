using System;
using SDWebImage;
using Foundation;
using CodeBucket.Core.Utils;
using CodeBucket;

// Analysis disable once CheckNamespace
namespace UIKit
{
    public static class UIImageViewExtensions
    {
        public static void SetAvatar(this UIImageView @this, Avatar avatar, int? size = 64)
        {
            @this.Image = Images.Avatar;

            if (avatar == null)
                return;

            var avatarUri = avatar.ToUri(size);
            if (avatarUri != null)
            {
                @this.SetImage(new NSUrl(avatarUri.AbsoluteUri), Images.Avatar, (img, err, type, imageUrl) => {
                    if (img == null || err != null)
                        return;

                    if (type == SDImageCacheType.None)
                    {
                        @this.Image = Images.Avatar;
                        @this.BeginInvokeOnMainThread(() =>
                            UIView.Transition(@this, 0.25f, UIViewAnimationOptions.TransitionCrossDissolve, () => @this.Image = img, null));
                    }
                });
            }
        }
    }
}

