using System;
using MonoTouch.UIKit;

namespace MonoTouch.UIKit
{
    public static class UIImageHelper
    {
        public static UIImage FromFileAuto(string filename, string extension = "png")
        {
            var retina = (UIScreen.MainScreen.Scale > 1.0);
            if (retina)
                return UIImage.FromFile(filename + "@2x." + extension);
            else
                return UIImage.FromFile(filename + "." + extension);
        }
    }
}

