using System;
using MonoTouch.Dialog.Utilities;
using BitbucketSharp.Models;
using MonoTouch.UIKit;

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
            ImageUri = new Uri(avatar);
        }
    }
}

