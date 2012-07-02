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
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(13);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);


        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public ChangeElement(ChangesetModel eventModel) : base(UITableViewCellStyle.Default, "changeelement")
        {
            Item = eventModel;
        }

        public ChangesetModel Item { get; set; }

        private string Message
        {
            get { return (Item.Message ?? "").Replace("\n", " ").Trim(); }
        }


        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            UIColor.Clear.SetFill();
            context.FillRect(bounds);

            var contentWidth = bounds.Width - LeftRightPadding * 2;

            var desc = Message;
            var user = Item.Author;

            UIColor.FromRGB(0, 0x44, 0x66).SetColor();
            view.DrawString(user,
                new RectangleF(LeftRightPadding, TopBottomPadding, contentWidth, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
                );


            string daysAgo = DateTime.Parse(Item.Utctimestamp).ToDaysAgo();
            UIColor.FromRGB(0.6f, 0.6f, 0.6f).SetColor();
            float daysAgoTop = TopBottomPadding + UserFont.LineHeight;
            view.DrawString(
                daysAgo,
                new RectangleF(LeftRightPadding,  daysAgoTop, contentWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                var top = daysAgoTop + DateFont.LineHeight + 2f;
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
            if (descHeight > (UserFont.LineHeight + 2) * 2)
                descHeight = (UserFont.LineHeight + 2) * 2;

            return TopBottomPadding*2 + UserFont.LineHeight + DateFont.LineHeight + 2f + descHeight;
        }
    }
}