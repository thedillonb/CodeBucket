using System;
using MonoTouch.Dialog.Utilities;
using MonoTouch.UIKit;
using CodeFramework.UI.Elements;

namespace CodeFramework.UI.Elements
{
    public class UserElement : SubcaptionElement, IImageUpdated
    {
        public static UIImage Default = null;

        public UserElement(string username, string firstName, string lastName, string avatar)
            : base (username)
        {
            var realName = firstName ?? "" + " " + lastName ?? "";
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

