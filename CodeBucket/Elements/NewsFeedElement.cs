using System;
using System.Drawing;
using MonoTouch.CoreGraphics;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Elements
{
    public class NewsFeedElement : NameTimeStringElement
    {
        public static UIImage DefaultImage;
        public static UIColor LinkColor;
        public static UIFont LinkFont;


        private TextBlock[] _blocks;
        private NSMutableAttributedString _string;
        private Kitty _kitty;
        private OHAttributedLabel _label;

        private UIImage LittleImage { get; set; }
        private float _lastHeight = 0;

        private List<ListToLinks> _listToLinks;
        private class ListToLinks
        {
            public NSRange Range;
            public NSAction Callback;
            public int Id;
        }

        public class TextBlock
        {
            public string Value;
            public NSAction Tapped;
            public UIFont Font;
            public UIColor Color;

            public TextBlock()
            {
            }

            public TextBlock(string value)
            {
                Value = value;
            }

            public TextBlock(string value, NSAction tapped = null)
            {
                Value = value;
                Tapped = tapped;
            }

            public TextBlock(string value, UIFont font = null, UIColor color = null, NSAction tapped = null)
            {
                Value = value; Font = font; Color = color; Tapped = tapped;
            }
        }

        public NewsFeedElement(string name, string imageUrl, DateTime time, IEnumerable<TextBlock> blocks, UIImage littleImage)
        {
            Lines = 4;
            _blocks = blocks.ToArray();
            _kitty = new Kitty { Parent = this };

            LittleImage = littleImage;
            Time = time;
            Name = name ?? "Unknown";

            Image = DefaultImage;
            if (imageUrl != null)
                ImageUri = new Uri(imageUrl);

            _string = new NSMutableAttributedString();
            _listToLinks = new List<ListToLinks>(_blocks.Length);

            int lengthCounter = 0;
            int i = 0;
            foreach (var b in _blocks)
            {
                UIColor color = null;
                if (b.Color != null)
                    color = b.Color;
                else
                {
                    if (b.Tapped != null)
                        color = LinkColor;
                }

                UIFont font = null;
                if (b.Font != null)
                    font = b.Font;
                else
                {
                    if (b.Tapped != null)
                        font = LinkFont;
                }

                if (color == null)
                    color = UIColor.Black;
                if (font == null)
                    font = UIFont.SystemFontOfSize(12f);


                var ctFont = new MonoTouch.CoreText.CTFont(font.Name, font.PointSize);
                var str = new NSAttributedString(b.Value, new MonoTouch.CoreText.CTStringAttributes() { ForegroundColor = color.CGColor, Font = ctFont });
                _string.Append(str);
                var strLength = str.Length;

                if (b.Tapped != null)
                    _listToLinks.Add(new ListToLinks { Range = new NSRange(lengthCounter, strLength), Callback = b.Tapped, Id = i++ });

                lengthCounter += strLength;
            }
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.AutosizesSubviews = true;

            foreach (var view in cell.Subviews)
            {
                if (view is OHAttributedLabel) {
                    view.RemoveFromSuperview();
                }
            }

            cell.AddSubview(_label);
            return cell;
        }

        public class Kitty : OHAttributedLabelDelegate
        {
            public NewsFeedElement Parent;
            public override bool ShouldFollowLink (OHAttributedLabel sender, NSObject linkInfo)
            {
                var a = (NSUrl)MonoTouch.ObjCRuntime.Runtime.GetNSObject (MonoTouch.ObjCRuntime.Messaging.IntPtr_objc_msgSend(linkInfo.Handle, MonoTouch.ObjCRuntime.Selector.GetHandle ("URL")));
                var id = Int32.Parse(a.AbsoluteString);
                Parent._listToLinks[id].Callback();
                return false;
            }
        }

        private void CreateOrUpdate(RectangleF frame)
        {
            if (_label == null)
            {
                _label = new OHAttributedLabel(frame);
                _label.Tag = 100;
                _label.BackgroundColor = UIColor.Clear;
                _label.AttributedText = _string;
                _label.Delegate = _kitty;
                _label.RemoveAllCustomLinks();
                _label.SetUnderlineLinks(false);
                _label.LineBreakMode = UILineBreakMode.WordWrap;
                if (LinkColor != null)
                    _label.LinkColor = LinkColor;

                foreach (var b in _listToLinks)
                {
                    _label.AddCustomLink(new NSUrl(b.Id.ToString()), b.Range);
                }

            }
            else
            {
                _label.Frame = frame;
            }
        }

        public override float Height(RectangleF bounds)
        {
            var f =  base.Height(bounds);

            if (bounds.Width != _lastHeight)
            {
                var width = bounds.Width;
                if (IsTappedAssigned)
                    width -= 20f;

                var newFrame = new RectangleF(LeftRightPadding, 45f, width - LeftRightPadding * 2, 0);
                CreateOrUpdate(newFrame);
                _label.SizeToFit();
                if (_label.Frame.Height > 60f)
                    CreateOrUpdate(new RectangleF(LeftRightPadding, 45f, width - LeftRightPadding * 2, 60));

                _label.SetNeedsDisplay();
                _lastHeight = bounds.Width;
            }

            return f + _label.Frame.Height;
        }

        public override void Draw(RectangleF bounds, CGContext context, UIView view)
        {
            base.Draw(bounds, context, view);
            if (LittleImage != null)
                LittleImage.Draw(new RectangleF(26, 26, 16f, 16f));
        }
    }
}