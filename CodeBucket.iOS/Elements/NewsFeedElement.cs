using System;
using System.Collections.Generic;
using Foundation;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Cells;
using Humanizer;

namespace CodeBucket.Elements
{
    public class NewsFeedElement : Element
    {
        private readonly string _time;
        private readonly Uri _imageUri;
        private readonly UIImage _actionImage;
        private readonly Action _tapped;

        private readonly NSMutableAttributedString _attributedHeader;
        private readonly NSMutableAttributedString _attributedBody;
        private readonly List<NewsCellView.Link> _headerLinks;
        private readonly List<NewsCellView.Link> _bodyLinks;

        public static UIColor LinkColor = Theme.CurrentTheme.MainTitleColor;

        private UIImage LittleImage { get; set; }

        public Action<NSUrl> WebLinkClicked;

        public class TextBlock
        {
            public string Value;
            public Action Tapped;
            public UIFont Font;
            public UIColor Color;

            public TextBlock()
            {
            }

            public TextBlock(string value)
            {
                Value = value;
            }

            public TextBlock(string value, Action tapped = null)
                : this (value)
            {
                Tapped = tapped;
            }

            public TextBlock(string value, UIFont font = null, UIColor color = null, Action tapped = null)
                : this(value, tapped)
            {
                Font = font; 
                Color = color;
            }
        }

        public NewsFeedElement(string imageUrl, DateTimeOffset time, IEnumerable<TextBlock> headerBlocks, IEnumerable<TextBlock> bodyBlocks, UIImage littleImage, Action tapped)
            : base(null)
        {
            Uri.TryCreate(imageUrl, UriKind.Absolute, out _imageUri);
            _time = time.Humanize();
            _actionImage = littleImage;
            _tapped = tapped;

            var header = CreateAttributedStringFromBlocks(headerBlocks);
            _attributedHeader = header.Item1;
            _headerLinks = header.Item2;

            var body = CreateAttributedStringFromBlocks(bodyBlocks);
            _attributedBody = body.Item1;
            _bodyLinks = body.Item2;
        }

        private static Tuple<NSMutableAttributedString,List<NewsCellView.Link>> CreateAttributedStringFromBlocks(IEnumerable<TextBlock> blocks)
        {
            var attributedString = new NSMutableAttributedString();
            var links = new List<NewsCellView.Link>();

            nint lengthCounter = 0;
            int i = 0;

            foreach (var b in blocks)
            {
                UIColor color = null;
                if (b.Color != null)
                    color = b.Color;
                else
                {
                    if (b.Tapped != null)
                        color = LinkColor;
                }

                var font = b.Font ?? UIFont.PreferredSubheadline;
                color = color ?? Theme.CurrentTheme.MainTextColor;
                var ctFont = new CoreText.CTFont(font.Name, font.PointSize);
                var str = new NSAttributedString(b.Value, new CoreText.CTStringAttributes { ForegroundColor = color.CGColor, Font = ctFont });
                attributedString.Append(str);
                var strLength = str.Length;

                if (b.Tapped != null)
                    links.Add(new NewsCellView.Link { Range = new NSRange(lengthCounter, strLength), Callback = b.Tapped, Id = i++ });

                lengthCounter += strLength;
            }

            return new Tuple<NSMutableAttributedString, List<NewsCellView.Link>>(attributedString, links);
        }

        public override UITableViewCell GetCell (UITableView tv)
        {
            var cell = tv.DequeueReusableCell(NewsCellView.Key) as NewsCellView;
            cell.Set(_imageUri, _time, _actionImage, _attributedHeader, _attributedBody, _headerLinks, _bodyLinks, WebLinkClicked);
            return cell;
        }

        public override void Selected(DialogViewController dvc, UITableView tableView, NSIndexPath path)
        {
            base.Selected(dvc, tableView, path);
            if (_tapped != null)
                _tapped();
            tableView.DeselectRow (path, true);
        }
    }
}