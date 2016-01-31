using System;
using UIKit;

namespace CodeBucket.Elements
{
    public class ButtonElement : StyledStringElement
    {
        public ButtonElement (string caption, string value, UIImage image = null) 
            : base (caption, value, UITableViewCellStyle.Value1) 
        {
            Image = image;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public ButtonElement (string caption, UIImage image = null) 
            : this (caption, null, image)
        {
        }
    }

}

