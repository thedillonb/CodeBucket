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
    public class ChangeElement : CustomElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(13);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(15);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);

        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public ChangeElement(ChangesetModel eventModel) : base(UITableViewCellStyle.Default, "changeelement")
        {
            Item = eventModel;
            BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("/Images/Cells/gradient"));
        }

        public ChangesetModel Item { get; set; }

        private string Message
        {
            get { return (Item.Message ?? "").Replace("\n", " ").Trim(); }
        }


        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            //UIColor.Clear.SetFill();
            //context.FillRect(bounds);

            var contentWidth = bounds.Width - LeftRightPadding * 2;

            var daysAgo = DateTime.Parse(Item.Utctimestamp).ToString("MM/dd/yy");
            UIColor.FromRGB(84, 135, 188).SetColor();
            var daysWidth = daysAgo.MonoStringLength(DateFont);
            view.DrawString(
                daysAgo,
                new RectangleF(bounds.Width - daysWidth - LeftRightPadding,  TopBottomPadding + 1f, daysWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );

            var desc = Message;
            var user = Item.Author;

            UIColor.FromRGB(41, 41, 41).SetColor();
            view.DrawString(user,
                new RectangleF(LeftRightPadding, TopBottomPadding, bounds.Width - daysWidth - LeftRightPadding * 2, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                var top = TopBottomPadding + UserFont.LineHeight + 2f;
                UIColor.FromRGB(120, 120, 120).SetColor();
                view.DrawString(desc,
                    new RectangleF(LeftRightPadding, top, contentWidth, bounds.Height - TopBottomPadding - top), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            var contentWidth = bounds.Width - LeftRightPadding * 2; //Account for the Accessory
            var desc = Message;
            var descHeight = desc.MonoStringHeight(DescFont, contentWidth);
            if (descHeight > (DescFont.LineHeight) * 3)
                descHeight = (DescFont.LineHeight) * 3;

            return TopBottomPadding*2 + UserFont.LineHeight + 2f + descHeight;
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            return cell;
        }

    }
}