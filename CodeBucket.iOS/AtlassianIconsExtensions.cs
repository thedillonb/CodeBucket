using System;
using UIKit;
using System.IO;
using Foundation;
using CodeBucket.Utils;

// Analysis disable once CheckNamespace
namespace CodeBucket
{
    public static class AtlassianIconsExtensions
    {
        private static readonly nfloat Scale;

        static AtlassianIconsExtensions()
        {
            Scale = UIScreen.MainScreen.Scale;
        }

        public static UIImage ToImage(this AtlassianIcon @this, nfloat size, bool cache = true)
        {
            var cacheDir = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.CachesDirectory, NSSearchPathDomain.User)[0].Path;

            string extension = string.Empty;
            if (Scale > 1 && Scale < 3)
            {
                extension = "@2x";
            }
            else if (Scale >= 3)
            {
                extension = "@3x";
            }

            var fileName = string.Format("octicon-{0}-{1}{2}.png", (int)@this.CharacterCode, size, extension);
            var combinedPath = Path.Combine(cacheDir, fileName);

            if (File.Exists(combinedPath))
            {
                var img = cache ? UIImage.FromBundle(combinedPath) : UIImage.FromFile(combinedPath);
                return img.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
            else
            {
                var img = Graphics.ImageFromFont(UIFont.FromName("octicons", size), @this.CharacterCode, UIColor.Black);
                if (img == null)
                    return null;
                var pngData = img.AsPNG();
                pngData.Save(combinedPath, false);
                return img.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            }
        }

        public static UIImage ToImage(this AtlassianIcon @this, bool cache = true)
        {
            return @this.ToImage(24f, cache);
        }

        public static UIImage ToEmptyListImage(this AtlassianIcon @this)
        {
            return @this.ToImage(64f, false);
        }
    }
}

