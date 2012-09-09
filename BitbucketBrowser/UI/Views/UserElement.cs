using System;
using MonoTouch.Dialog.Utilities;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using CodeFramework.UI.Elements;

namespace BitbucketBrowser.UI
{
    public class UserElement : SubcaptionElement, IImageUpdated
    {
        public UserElement(string username, string firstName, string lastName, string avatar)
            : base (username)
        {
            var realName = firstName ?? "" + " " + lastName ?? "";
             if (!string.IsNullOrWhiteSpace(realName))
                Value = realName;
            Accessory = UITableViewCellAccessory.DisclosureIndicator;
            Image = Images.Anonymous;
            if (avatar != null)
                ImageUri = new Uri(avatar);
        }
    }
}

