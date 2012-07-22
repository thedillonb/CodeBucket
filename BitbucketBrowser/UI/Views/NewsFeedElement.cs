using System;
using System.Drawing;
using BitbucketBrowser.Utils;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MonoTouch.Dialog.Utilities;

namespace BitbucketBrowser.UI
{
    public class NewsFeedElement : NameTimeStringElement
    {
        public NewsFeedElement(EventModel eventModel)
        {
            Item = eventModel;
            ReportUser = true;
            ReportRepository = false;
            Lines = 4;


            string desc;
            UIImage img;
            CreateDescription(out desc, out img);

            String = desc;
            LittleImage = img;
            Name = eventModel.User.Username;
            Time = eventModel.UtcCreatedOn;
            Image = Images.Anonymous;
            ImageUri = new Uri(eventModel.User.Avatar);
        }

        public EventModel Item { get; set; }

        public bool ReportUser { get; set; }

        public bool ReportRepository { get; set; }

        private UIImage LittleImage { get; set; }


        public static List<string> SupportedEvents = new List<string> { EventModel.Type.Commit, EventModel.Type.CreateRepo, EventModel.Type.WikiUpdated, EventModel.Type.WikiCreated,
                                                    EventModel.Type.StartFollowRepo, EventModel.Type.StartFollowUser, EventModel.Type.StopFollowRepo };

        private void CreateDescription(out string desc, out UIImage img)
        {
            desc = string.IsNullOrEmpty(Item.Description) ? "" : Item.Description.Replace("\n", " ").Trim();

            //Drop the image
            if (Item.Event == EventModel.Type.Commit)
            {
                img = Images.Plus;
                if (ReportRepository)
                    desc = "Commit to " + Item.Repository.Name + ": " + desc;
                else
                    desc = "Commited: " + desc;
            }
            else if (Item.Event == EventModel.Type.CreateRepo)
            {
                img = Images.Create;
                if (ReportRepository)
                    desc = "Created Repo: " + Item.Repository.Name;
                else
                    desc = "Repository Created";
            }
            else if (Item.Event == EventModel.Type.WikiUpdated)
            {
                img = Images.Pencil;
                desc = "Updated the wiki page: " + desc;
            }
            else if (Item.Event == EventModel.Type.WikiCreated)
            {
                img = Images.Pencil;
                desc = "Created the wiki page: " + desc;
            }
            else if (Item.Event == EventModel.Type.StartFollowUser)
            {
                img = Images.HeartAdd;
                desc = "Started following a user";
            }
            else if (Item.Event == EventModel.Type.StartFollowRepo)
            {
                img = Images.HeartAdd;
                desc = "Started following: " + Item.Repository.Name;
            }
            else if (Item.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                desc = "Stopped following: " + Item.Repository.Name;
            }
            else
                img = Images.Unknown;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            base.Draw(bounds, context, view);
            if (LittleImage != null)
                LittleImage.Draw(new RectangleF(26, 26, 16f, 16f));

        }
    }
}