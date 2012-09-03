using System;
using System.Drawing;
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
        public NewsFeedElement(EventModel eventModel, bool reportRepo = true)
        {
            Item = eventModel;
            ReportRepository = reportRepo;
            Lines = 4;


            string desc;
            UIImage img;
            CreateDescription(out desc, out img);

            String = desc;
            LittleImage = img;
            Time = eventModel.UtcCreatedOn;
            Image = Images.Anonymous;

            //Unreal.. User can be null!? Fucking retarded.
            if (eventModel.User != null)
            {
                Name = eventModel.User.Username;
                ImageUri = new Uri(eventModel.User.Avatar);
            }
            else
            {
                Name = "Unknown";
            }
        }

        public EventModel Item { get; set; }

        private bool ReportRepository { get; set; }

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
                    desc = "Commit to " + repoName() + ":\n" + desc;
                else
                    desc = "Commited: " + desc;
            }
            else if (Item.Event == EventModel.Type.CreateRepo)
            {
                img = Images.Create;
                if (ReportRepository)
                    desc = "Created Repo: " + repoName();
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
                desc = "Started following: " + repoName();
            }
            else if (Item.Event == EventModel.Type.StopFollowRepo)
            {
                img = Images.HeartDelete;
                desc = "Stopped following: " + repoName();
            }
            else
                img = Images.Priority;
        }

        private string repoName()
        {
            if (!Item.Repository.Owner.ToLower().Equals(Application.Account.Username.ToLower()))
                return Item.Repository.Owner + "/" + Item.Repository.Name;
            return Item.Repository.Name;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            base.Draw(bounds, context, view);
            if (LittleImage != null)
                LittleImage.Draw(new RectangleF(26, 26, 16f, 16f));

        }
    }
}