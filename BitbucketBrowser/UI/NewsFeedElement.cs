using System;
using MonoTouch.Dialog;
using BitbucketSharp.Models;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace BitbucketBrowser.UI
{
	public class NewsFeedElement : OwnerDrawnElement
	{
		private static UIFont DateFont = UIFont.SystemFontOfSize(13);
		private static UIFont UserFont = UIFont.SystemFontOfSize(15);
		private static UIFont DescFont = UIFont.SystemFontOfSize(12);
		private static UIImage plusImage = UIImage.FromBundle("Images/plus.png");
		private static UIImage heartImage = UIImage.FromBundle("Images/heart.png");
		private static UIImage pencilImage = UIImage.FromBundle("Images/pencil.png");
		private static UIImage unknownImage = UIImage.FromBundle("Images/unknown.png");
		
		private float leftRightPadding = 6f;
		private float topBottomPadding = 6f;
		
		public EventModel Item { get; set; }
		
		public NewsFeedElement(EventModel eventModel) : base(UITableViewCellStyle.Default, "newsfeedelement")
		{
			Item = eventModel;
		}

		public override void Draw(System.Drawing.RectangleF bounds, MonoTouch.CoreGraphics.CGContext context, MonoTouch.UIKit.UIView view)
		{
			UIColor.White.SetFill();
			context.FillRect(bounds);
			
			var days = DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days;
			var daysAgo = days > 0 ? DateTime.Now.Subtract(DateTime.Parse(Item.CreatedOn)).Days + " days ago" : "Today";
			UIColor.FromRGB(36, 112, 216).SetColor();
			var daysAgoWidth = daysAgo.MonoStringLength(DateFont) + leftRightPadding + 5;
			view.DrawString(
				daysAgo,
				new RectangleF(bounds.Width - daysAgoWidth, topBottomPadding + 1f, daysAgoWidth, DateFont.LineHeight),
				DateFont,
				UILineBreakMode.TailTruncation
			);
			
			var imageRect = new RectangleF(leftRightPadding, topBottomPadding + 1, 16f, 16f);

			//Drop the image
			if (Item.Event == "commit")
				plusImage.Draw(imageRect);
			else if (Item.Event == "wiki_updated")
				pencilImage.Draw(imageRect);
			else if (Item.Event == "start_follow_user")
				heartImage.Draw(imageRect);
			else
				unknownImage.Draw(imageRect);
			
			string userStr = "";
			if (!EventModel.EventToString.TryGetValue(Item.Event, out userStr))
				userStr = "Unknown";
			UIColor.Black.SetColor();
			view.DrawString(
				userStr,
				new RectangleF(leftRightPadding + 16f + 6f, topBottomPadding, bounds.Width - daysAgoWidth - 24f, UserFont.LineHeight),
				UserFont,
				UILineBreakMode.TailTruncation
			);
			
			if (!string.IsNullOrEmpty(Item.Description))
			{
				var descStr = Item.Description.Replace("\n", "");
				UIColor.Black.SetColor();
				var descWidth = bounds.Width - leftRightPadding * 2;
				var descStrHeight = descStr.MonoStringHeight(DescFont, descWidth);
				if (descStrHeight > 34)
					descStrHeight = 34;
				view.DrawString(
					descStr,
					new RectangleF(leftRightPadding, topBottomPadding + UserFont.LineHeight + 6f, descWidth, descStrHeight),
					DescFont,
					UILineBreakMode.TailTruncation
				);
			}
			
		}
		
		public override float Height(RectangleF bounds)
		{
			var descHeight = 0f;
			var descWidth = bounds.Width - leftRightPadding * 2;
			if (!string.IsNullOrEmpty(Item.Description))
			{
				descHeight = Item.Description.Replace("\n", "").MonoStringHeight(DescFont, descWidth);
				if (descHeight > 34)
					descHeight = 34;
				descHeight += 6f;
			}
			
			return topBottomPadding * 2 + UserFont.LineHeight + descHeight;
		}

	}
}

