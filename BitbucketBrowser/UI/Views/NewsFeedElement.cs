using System;
using System.Drawing;
using BitbucketBrowser.Utils;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace BitbucketBrowser.UI
{
    public class NewsFeedElement : OwnerDrawnElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(13);
        private static readonly UIFont UserFont = UIFont.SystemFontOfSize(15);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(12);
        private static readonly UIImage PlusImage = UIImage.FromBundle("Images/plus.png");
        private static readonly UIImage HeartImage = UIImage.FromBundle("Images/heart.png");
        private static readonly UIImage PencilImage = UIImage.FromBundle("Images/pencil.png");
        private static readonly UIImage UnknownImage = UIImage.FromBundle("Images/unknown.png");

        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public NewsFeedElement(EventModel eventModel) : base(UITableViewCellStyle.Default, "newsfeedelement")
        {
            Item = eventModel;
        }

        public EventModel Item { get; set; }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            UIColor.White.SetFill();
            context.FillRect(bounds);

            int days = DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days;
            string daysAgo = days > 0
                                 ? DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days + " days ago"
                                 : "Today";
            UIColor.FromRGB(36, 112, 216).SetColor();
            float daysAgoWidth = daysAgo.MonoStringLength(DateFont) + LeftRightPadding + 5;
            view.DrawString(
                daysAgo,
                new RectangleF(bounds.Width - daysAgoWidth, TopBottomPadding + 1f, daysAgoWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );

            var imageRect = new RectangleF(LeftRightPadding, TopBottomPadding + 1, 16f, 16f);

            //Drop the image
            if (Item.Event == "commit")
                PlusImage.Draw(imageRect);
            else if (Item.Event == "wiki_updated")
                PencilImage.Draw(imageRect);
            else if (Item.Event == "start_follow_user")
                HeartImage.Draw(imageRect);
            else
                UnknownImage.Draw(imageRect);

            string userStr;
            if (!EventModel.EventToString.TryGetValue(Item.Event, out userStr))
                userStr = "Unknown";
            UIColor.Black.SetColor();
            view.DrawString(
                userStr,
                new RectangleF(LeftRightPadding + 16f + 6f, TopBottomPadding, bounds.Width - daysAgoWidth - 24f,
                               UserFont.LineHeight),
                UserFont,
                UILineBreakMode.TailTruncation
                );

            if (!string.IsNullOrEmpty(Item.Description))
            {
                string descStr = Item.Description.Replace("\n", "");
                UIColor.Black.SetColor();
                float descWidth = bounds.Width - LeftRightPadding*2;
                float descStrHeight = descStr.MonoStringHeight(DescFont, descWidth);
                if (descStrHeight > 34)
                    descStrHeight = 34;
                view.DrawString(
                    descStr,
                    new RectangleF(LeftRightPadding, TopBottomPadding + UserFont.LineHeight + 6f, descWidth,
                                   descStrHeight),
                    DescFont,
                    UILineBreakMode.TailTruncation
                    );
            }
        }

        public override float Height(RectangleF bounds)
        {
            float descHeight = 0f;
            float descWidth = bounds.Width - LeftRightPadding*2;
            if (!string.IsNullOrEmpty(Item.Description))
            {
                descHeight = Item.Description.Replace("\n", "").MonoStringHeight(DescFont, descWidth);
                if (descHeight > 34)
                    descHeight = 34;
                descHeight += 6f;
            }

            return TopBottomPadding*2 + UserFont.LineHeight + descHeight;
        }
    }
}