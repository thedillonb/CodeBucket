using System;
using System.Drawing;
using BitbucketSharp.Models;
using MonoTouch.CoreGraphics;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using MonoTouch.Dialog.Utilities;
using CodeFramework.UI.Elements;
using CodeFramework.UI.Views;

namespace BitbucketBrowser.UI
{
    public class NewsFeedElement : NameTimeStringElement
    {
        InteractiveTextView _textView;
        InteractiveTextView.TextBlock[] _blocks;

        public NewsFeedElement(EventModel eventModel, InteractiveTextView.TextBlock[] blocks, UIImage littleImage, bool reportRepo = true)
        {
            Item = eventModel;
            ReportRepository = reportRepo;
            Lines = 4;
            _blocks = blocks;

            LittleImage = littleImage;
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

        public override UITableViewCell GetCell(UITableView tv)
        {

            var cell = base.GetCell(tv);



            foreach (var view in cell.Subviews)
            {
                if (view is InteractiveTextView) {
                    view.RemoveFromSuperview();
//                    ((InteractiveTextView)view).textBlocks = _blocks;
//                    ((InteractiveTextView)view).DoLayout();
//                    ((InteractiveTextView)view).SetNeedsDisplay();
                }
            }

            cell.AddSubview(_textView);


            return cell;
        }

        public override float Height(RectangleF bounds)
        {
            var f =  base.Height(bounds);

            if (_textView == null)
            {
                var width = bounds.Width;
                if (IsTappedAssigned)
                    width -= 20f;
                _textView = new InteractiveTextView(new RectangleF(LeftRightPadding, 45f, width - LeftRightPadding * 2, 200f - 40f), _blocks);
            }

            return f + _textView.Height;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            base.Draw(bounds, context, view);
            if (LittleImage != null)
                LittleImage.Draw(new RectangleF(26, 26, 16f, 16f));
        }
    }
}