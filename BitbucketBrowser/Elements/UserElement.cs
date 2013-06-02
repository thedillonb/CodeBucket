using System;
using MonoTouch.UIKit;

namespace CodeBucket.Elements
{
    public class UserElement : SubcaptionElement
    {
        public static UIImage Default;

        public UserElement(string username, string firstName, string lastName, string avatar)
            : base (username)
        {
            var realName = firstName ?? "" + " " + (lastName ?? "");
             if (!string.IsNullOrWhiteSpace(realName))
                Value = realName;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
            if (Default != null)
                Image = Default;
            if (avatar != null)
                ImageUri = new Uri(avatar);
        }
    }
}

