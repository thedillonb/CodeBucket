using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using SDWebImage;

namespace CodeBucket.TableViewCells
{
    public partial class NewsCellView : UITableViewCell
    {
        public static readonly UINib Nib = UINib.FromName("NewsCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NewsCellView");
        private static nfloat DefaultContentConstraint = 0f;

        public class Link
        {
            public NSRange Range;
            public Action Callback;
            public int Id;
        }

        public NewsCellView(IntPtr handle) : base(handle)
        {
        }

        public static NewsCellView Create()
        {
            return (NewsCellView)Nib.Instantiate(null, null)[0];
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Image.Layer.MasksToBounds = true;
            Image.Layer.CornerRadius = Image.Bounds.Height / 2f;

            Body.LinkAttributes = new NSDictionary();
            Body.ActiveLinkAttributes = new NSMutableDictionary();
            Body.ActiveLinkAttributes[CoreText.CTStringAttributeKey.UnderlineStyle] = NSNumber.FromBoolean(true);

            Header.LinkAttributes = new NSDictionary();
            Header.ActiveLinkAttributes = new NSMutableDictionary();
            Header.ActiveLinkAttributes[CoreText.CTStringAttributeKey.UnderlineStyle] = NSNumber.FromBoolean(true);

            DefaultContentConstraint = ContentConstraint.Constant;

            ActionImage.TintColor = Time.TextColor;
        }

        class LabelDelegate : MonoTouch.TTTAttributedLabel.TTTAttributedLabelDelegate {

            private readonly List<Link> _links;
            private readonly Action<NSUrl> _webLinkClicked;

            public LabelDelegate(List<Link> links, Action<NSUrl> webLinkClicked)
            {
                _links = links;
                _webLinkClicked = webLinkClicked;
            }

            public override void DidSelectLinkWithURL (MonoTouch.TTTAttributedLabel.TTTAttributedLabel label, NSUrl url)
            {
                try
                {
                    if (url.ToString().StartsWith("http", StringComparison.Ordinal))
                    {
                        if (_webLinkClicked != null)
                            _webLinkClicked(url);
                    }
                    else
                    {
                        var i = Int32.Parse(url.ToString());
                        _links[i].Callback();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to callback on TTTAttributedLabel: {0}", e.Message);
                }
            }
        }

        public void Set(Uri imageUri, string time, UIImage actionImage, 
            NSMutableAttributedString header, NSMutableAttributedString body, 
            List<Link> headerLinks, List<Link> bodyLinks, Action<NSUrl> webLinkClicked,
            bool multilined)
        {
            Time.Text = time;
            ActionImage.Image = actionImage;
            Body.Hidden = body.Length == 0;
            Body.Lines = multilined ? 0 : 4;
            ContentConstraint.Constant = Body.Hidden ? 0 : DefaultContentConstraint;

            if (imageUri != null)
            {
                Image.SetImage(new NSUrl(imageUri.AbsoluteUri), Images.Avatar);
            }
            else
            {
                Image.Image = null;
            }

            if (header == null)
                header = new NSMutableAttributedString();
            if (body == null)
                body = new NSMutableAttributedString();

            Header.AttributedText = header;
            Header.Delegate = new LabelDelegate(headerLinks, webLinkClicked);

            Body.AttributedText = body;
            Body.Hidden = body.Length == 0;
            Body.Delegate = new LabelDelegate(bodyLinks, webLinkClicked);

            foreach (var b in headerLinks)
                Header.AddLinkToURL(new NSUrl(b.Id.ToString()), b.Range);

            foreach (var b in bodyLinks)
                Body.AddLinkToURL(new NSUrl(b.Id.ToString()), b.Range);
        }
    }
}

