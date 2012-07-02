using System;
using System.Drawing;
using BitbucketBrowser.Utils;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace BitbucketBrowser.UI
{
    public class NewsFeedElement : CustomElement
    {
        private static readonly UIFont DateFont = UIFont.SystemFontOfSize(12);
        private static readonly UIFont UserFont = UIFont.BoldSystemFontOfSize(13);
        private static readonly UIFont DescFont = UIFont.SystemFontOfSize(14);
        private static readonly UIImage PlusImage = UIImage.FromBundle("Images/plus.png");
        private static readonly UIImage HeartImage = UIImage.FromBundle("Images/heart.png");
        private static readonly UIImage PencilImage = UIImage.FromBundle("Images/pencil.png");
        private static readonly UIImage UnknownImage = UIImage.FromBundle("Images/unknown.png");
        private static readonly UIImage HeartBrokenImage = UIImage.FromBundle("Images/heart_break.png");
        private static readonly UIImage CreateImage = UIImage.FromBundle("Images/create.png");

        private const float LeftRightPadding = 6f;
        private const float TopBottomPadding = 6f;

        public NewsFeedElement(EventModel eventModel) : base(UITableViewCellStyle.Default, "newsfeedelement")
        {
            Item = eventModel;
            ReportUser = true;
        }

        public EventModel Item { get; set; }

        public bool ReportUser { get; set; }


        public static List<string> SupportedEvents = new List<string> { EventModel.Type.Commit, EventModel.Type.CreateRepo, EventModel.Type.WikiUpdated, EventModel.Type.WikiCreated,
                                                    EventModel.Type.StartFollowRepo, EventModel.Type.StartFollowUser, EventModel.Type.StopFollowRepo };

        private void CreateDescription(out string desc, out UIImage img)
        {
            desc = string.IsNullOrEmpty(Item.Description) ? "" : Item.Description.Replace("\n", " ").Trim();

            //Drop the image
            if (Item.Event == EventModel.Type.Commit)
            {
                img = PlusImage;
                desc = "Commited: " + desc;
            }
            else if (Item.Event == EventModel.Type.CreateRepo)
            {
                img = CreateImage;
                desc = "Created Repo: " + Item.Repository.Name;
            }
            else if (Item.Event == EventModel.Type.WikiUpdated)
            {
                img = PencilImage;
                desc = "Updated the wiki page: " + desc;
            }
                else if (Item.Event == EventModel.Type.WikiCreated)
            {
                img = PencilImage;
                desc = "Created the wiki page: " + desc;
            }
            else if (Item.Event == EventModel.Type.StartFollowUser)
            {
                img = HeartImage;
                desc = "Started following a user";
            }
            else if (Item.Event == EventModel.Type.StartFollowRepo)
            {
                img = HeartImage;
                desc = "Started following: " + Item.Repository.Name;
            }
            else if (Item.Event == EventModel.Type.StopFollowRepo)
            {
                img = HeartBrokenImage;
                desc = "Stopped following: " + Item.Repository.Name;
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

            if (ReportUser)
            {
                var user = Item.User != null ? Item.User.Username : Item.Repository.Owner;
                UIColor.FromRGB(0, 0x44, 0x66).SetColor();
                view.DrawString(user,
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
            float daysAgoTop = TopBottomPadding + userHeight;
            view.DrawString(
                daysAgo,
                new RectangleF(leftContent,  daysAgoTop, contentWidth, DateFont.LineHeight),
                DateFont,
                UILineBreakMode.TailTruncation
                );


            if (!string.IsNullOrEmpty(desc))
            {
                UIColor.Black.SetColor();
                var top = daysAgoTop + DateFont.LineHeight + 2f;
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

            var userHeight = (ReportUser) ? UserFont.LineHeight : 0f;

            return TopBottomPadding*2 + userHeight + DateFont.LineHeight + 2f + descHeight;
        }
    }
}