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
    public class NewsFeedElement : OwnerDrawnElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.SystemFontOfSize(15);
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
        }

        public EventModel Item { get; set; }

        public event NSAction Tapped;

        private void CreateDescription(out string desc, out UIImage img)
        {
            desc = string.IsNullOrEmpty(Item.Description) ? "" : Item.Description.Replace("\n", " ");

            //Drop the image
            if (Item.Event == "commit")
            {
                img = PlusImage;
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
            UIColor.White.SetFill();
            context.FillRect(bounds);

            var imageRect = new RectangleF(LeftRightPadding, bounds.Height / 2 - 8f, 16f, 16f);
            var leftContent = LeftRightPadding * 2 + imageRect.Width;
            var contentWidth = bounds.Width - leftContent - LeftRightPadding;

            string desc = null;
            UIImage img = null;
            CreateDescription(out desc, out img);

            img.Draw(imageRect);

            string userStr;
            if (!EventModel.EventToString.TryGetValue(Item.Event, out userStr))
                userStr = "Unknown";
            UIColor.Black.SetColor();
            view.DrawString(userStr,
                new RectangleF(leftContent, TopBottomPadding, contentWidth, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
                );

            int days = DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days;
            string daysAgo = days > 0 ? DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days + " days ago" : "Today";
            UIColor.FromRGB(36, 112, 216).SetColor();
            float daysAgoTop = TopBottomPadding + UserFont.LineHeight + 2;
            view.DrawString(
                daysAgo,
                new RectangleF(leftContent,  daysAgoTop, contentWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                float descStrHeight = desc.MonoStringHeight(DescFont, contentWidth);
                if (descStrHeight > 34)
                    descStrHeight = 34;
                view.DrawString(desc,
                    new RectangleF(leftContent, daysAgoTop + DateFont.LineHeight + TopBottomPadding, contentWidth,
                                   descStrHeight), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            float descHeight = 0f;
            float descWidth = bounds.Width - LeftRightPadding*2;
            string desc = null;
            UIImage img = null;
            CreateDescription(out desc, out img);

            descHeight = desc.MonoStringHeight(DescFont, descWidth);
            if (descHeight > 34)
                descHeight = 34;

            return TopBottomPadding*3 + UserFont.LineHeight + DateFont.LineHeight + 2f + descHeight;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell =  base.GetCell(tv);

            if (Tapped != null) {
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
            } 
            else 
            {
                cell.Accessory = UITableViewCellAccessory.None;
                cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            }
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (Tapped != null)
                Tapped();
            tableView.DeselectRow (path, true);
        }
    }
}