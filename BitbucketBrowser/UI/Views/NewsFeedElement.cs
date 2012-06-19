using System;
using System.Drawing;
using BitbucketBrowser.Utils;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{
    public class NewsFeedElement : CustomElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.SystemFontOfSize(13);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);
        private static readonly UIImage PlusImage = UIImage.FromBundle("Images/plus.png");
        private static readonly UIImage HeartImage = UIImage.FromBundle("Images/heart.png");
        private static readonly UIImage PencilImage = UIImage.FromBundle("Images/pencil.png");
        private static readonly UIImage UnknownImage = UIImage.FromBundle("Images/unknown.png");

        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public NewsFeedElement(EventModel eventModel) : base(UITableViewCellStyle.Default, "newsfeedelement")
        {
            Item = eventModel;
            ReportUser = true;
        }

        public EventModel Item { get; set; }

        public bool ReportUser { get; set; }

        private void CreateDescription(out string desc, out UIImage img)
        {
            desc = string.IsNullOrEmpty(Item.Description) ? "" : Item.Description.Replace("\n", " ").Trim();

            //Drop the image
            if (Item.Event == "commit")
            {
                img = PlusImage;
                desc = "Commited: " + desc;
            }
            else if (Item.Event == "wiki_updated")
            {
                img = PencilImage;
                desc = "Updated the wiki page: " + desc;
            }
            else if (Item.Event == "start_follow_user")
            {
                img = HeartImage;
                desc = "Started following a user";
            }
            else if (Item.Event == "wiki_created")
            {
                img = PencilImage;
                desc = "Created the wiki page: " + desc;
            }
            else
                img = UnknownImage;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            UIColor.Clear.SetFill();
            context.FillRect(bounds);

            var imageRect = new RectangleF(LeftRightPadding, bounds.Height / 2 - 8f, 16f, 16f);
            var leftContent = LeftRightPadding * 2 + imageRect.Width;
            var contentWidth = bounds.Width - leftContent - LeftRightPadding;
            var userHeight = UserFont.LineHeight;

            string desc = null;
            UIImage img = null;
            CreateDescription(out desc, out img);

            img.Draw(imageRect);

            if (ReportUser && Item.User != null)
            {
                string userStr = Item.User.Username;
                UIColor.FromRGB(0, 0x44, 0x66).SetColor();
                view.DrawString(userStr,
                    new RectangleF(leftContent, TopBottomPadding, contentWidth, UserFont.LineHeight),
                    UserFont, UILineBreakMode.TailTruncation
                    );
            }
            else
            {
                userHeight = -2;
            }

            string daysAgo = DateTime.Parse(Item.UtcCreatedOn).ToDaysAgo();
            UIColor.FromRGB(0.6f, 0.6f, 0.6f).SetColor();
            float daysAgoTop = TopBottomPadding + userHeight + 2;
            view.DrawString(
                daysAgo,
                new RectangleF(leftContent,  daysAgoTop, contentWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                var top = daysAgoTop + DateFont.LineHeight + 3f;
                var height = bounds.Height - top - TopBottomPadding;
                view.DrawString(desc,
                    new RectangleF(leftContent, top, contentWidth, height), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            float descHeight = 0f;
            var leftContent = LeftRightPadding * 2 + 16f;
            var contentWidth = bounds.Width - 20f - leftContent - LeftRightPadding; //Account for the Accessory
            string desc = null;
            UIImage img = null;
            CreateDescription(out desc, out img);

            descHeight = desc.MonoStringHeight(DescFont, contentWidth);
            if (descHeight > 54)
                descHeight = 54;

            var userHeight = (ReportUser && Item.User != null) ? UserFont.LineHeight : 0f;

            return TopBottomPadding*2 + 3f + userHeight + DateFont.LineHeight + 2f + descHeight;
        }
    }
}