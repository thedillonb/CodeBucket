using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace CodeFramework.UI.Elements
{
    public class StyledElement : MonoTouch.Dialog.StyledStringElement
    {
        public static UIFont  DefaultTitleFont = UIFont.BoldSystemFontOfSize(15f);
        public static UIFont  DefaultDetailFont = UIFont.SystemFontOfSize(15f);
        public static UIColor DefaultTitleColor = UIColor.FromRGB(41, 41, 41);
        public static UIColor DefaultDetailColor = UIColor.FromRGB(120, 120, 120);
        public static UIColor BgColor;

        public StyledElement(string title)
            : base(title)
        {
            Init();
        }

        public StyledElement(string title, string subtitle, UITableViewCellStyle style)
            : base(title, subtitle, style)
        {
            Init();
        }

        public StyledElement(string title, string subtitle)
            : base(title, subtitle)
        {
            Init();
        }

        public StyledElement(string title, NSAction action)
            : base(title, action)
        {
            Init();
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public StyledElement (string caption,  NSAction tapped, UIImage image) 
            : base (caption, tapped)
        {
            Init();
            Image = image;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public StyledElement(string caption, UIImage image)
            : this(caption)
        {
            Init();
            Image = image;
        }


        private void Init()
        {
            Font = DefaultTitleFont;
            SubtitleFont = DefaultDetailFont;
            this.BackgroundColor = BgColor;
            this.TextColor = DefaultTitleColor;
            this.DetailColor = DefaultDetailColor;
            LineBreakMode = MonoTouch.UIKit.UILineBreakMode.TailTruncation;
            Lines = 1;
        }
    }
}

