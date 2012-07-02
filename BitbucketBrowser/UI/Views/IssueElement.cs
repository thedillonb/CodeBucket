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
    public class IssueElement : CustomElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(13);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);

        public static UIImage BugImage = UIImage.FromBundle("Images/Issues/bug.png");
        public static UIImage TaskImage = UIImage.FromBundle("Images/Issues/task.png");
        public static UIImage EnhancementImage = UIImage.FromBundle("Images/Issues/enhancement.png");
        public static UIImage ProposalImage = UIImage.FromBundle("Images/Issues/proposal.png");

//        private static UIImage BlockImage = UIImage.FromBundle("Images/unknown.png");
//        private static UIImage TrivialImage = UIImage.FromBundle("Images/Issues/low.png");
//        private static UIImage MajorImage = UIImage.FromBundle("Images/Issues/high.png");



        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public IssueElement(IssueModel eventModel) : base(UITableViewCellStyle.Default, "issueelement")
        {
            Item = eventModel;
            Message = Item.Content.OneLine();
        }

        private IssueModel Item { get; set; }
        private string Message { get; set; }

        public override bool Matches(string text)
        {
            var lowerText = text.ToLower();
            return Item.LocalId.ToString().ToLower().Equals(lowerText) || Item.Title.ToLower().Contains(lowerText);
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            UIColor.Clear.SetFill();
            context.FillRect(bounds);

            var leftOffset = LeftRightPadding * 2 + BugImage.Size.Width;
            var contentWidth = bounds.Width - leftOffset - LeftRightPadding;
            var img2left = bounds.Width - LeftRightPadding - BugImage.Size.Width;

            UIImage img1 = null;
            if (Item.Metadata.Kind == "bug")
                img1 = BugImage;
            else if (Item.Metadata.Kind == "enhancement")
                img1 = EnhancementImage;
            else if (Item.Metadata.Kind == "task")
                img1 = TaskImage;
            else if (Item.Metadata.Kind == "proposal")
                img1 = ProposalImage;
            
            if (img1 != null)
                img1.Draw(new RectangleF(LeftRightPadding, bounds.Height / 2 - BugImage.Size.Height / 2, BugImage.Size.Width, BugImage.Size.Height));

            //Priority
            /*
            UIImage img2 = null;
            if (Item.Priority == "blocker")
                img2 = BlockImage;
            else if (Item.Priority == "trivial" || Item.Priority == "minor")
                img2 = TrivialImage;
            else if (Item.Priority == "major" || Item.Priority == "critical")
                img2 = MajorImage;
            
            
            if (img2 != null)
                img2.Draw(new RectangleF(img2left, TopBottomPadding + 1f, BlockImage.Size.Width, BlockImage.Size.Height));
            */


            UIColor.FromRGB(0, 0x44, 0x66).SetColor();
            view.DrawString("#" + Item.LocalId + ": " + Item.Title,
                new RectangleF(leftOffset, TopBottomPadding, img2left - LeftRightPadding, UserFont.LineHeight),
                UserFont, UILineBreakMode.TailTruncation
                );

            UIColor.FromRGB(0.6f, 0.6f, 0.6f).SetColor();

            var daysAgo = DateTime.Parse(Item.UtcCreatedOn).ToDaysAgo();
            var daysAgoTop = TopBottomPadding + UserFont.LineHeight;

            //Draw the status

            var statusWidth = 0.0f; //Item.Status.MonoStringLength(DateFont);
            //view.DrawString(Item.Status, new RectangleF(bounds.Width - LeftRightPadding - statusWidth, daysAgoTop, statusWidth, DateFont.LineHeight), DateFont, UILineBreakMode.TailTruncation);

            //Draw the number of days ago
            view.DrawString(
                daysAgo,
                new RectangleF(leftOffset,  daysAgoTop, contentWidth - statusWidth - 6f, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(Message))
            {
                UIColor.Black.SetColor();
                var top = daysAgoTop + DateFont.LineHeight + 2f;
                view.DrawString(Message,
                    new RectangleF(leftOffset, top, contentWidth, bounds.Height - TopBottomPadding - top), DescFont, UILineBreakMode.TailTruncation
                );
            }
        }

        public override float Height(RectangleF bounds)
        {
            var contentWidth = bounds.Width - LeftRightPadding * 2; //Account for the Accessory
            float descHeight = 0f;

            if (!string.IsNullOrEmpty(Message))
            {
                descHeight = Message.MonoStringHeight(DescFont, contentWidth);
                if (descHeight > (DescFont.LineHeight + 2) * 2)
                    descHeight = (DescFont.LineHeight + 2) * 2;
            }

            return TopBottomPadding*2 + UserFont.LineHeight + DateFont.LineHeight + 2f + descHeight;
        }
    }
}