using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CodeBucket.Core.ViewModels.Events;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Utils;
using System.Linq;
using CodeBucket.Client.V1;

namespace CodeBucket.TableViewCells
{
    [Register("UILabelWithLinks")]
    public class UILabelWithLinks : Xamarin.TTTAttributedLabel.TTTAttributedLabel
    {
        public UILabelWithLinks(IntPtr ptr)
            : base(ptr)
        {
        }

        public UILabelWithLinks(NSCoder coder)
            : base(coder)
        {
        }
    }

    public partial class NewsCellView : BaseTableViewCell<EventItemViewModel>
    {
        public static readonly UINib Nib = UINib.FromName("NewsCellView", NSBundle.MainBundle);
        public static readonly NSString Key = new NSString("NewsCellView");
        private static nfloat DefaultContentConstraint = 0f;

        public static UIColor LinkColor = Theme.CurrentTheme.MainTitleColor;

        public class Link
        {
            public NSRange Range;
            public Action Callback;
            public int Id;
        }

        public NewsCellView(IntPtr handle) 
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Image.Layer.MasksToBounds = true;
            Image.Layer.CornerRadius = Image.Bounds.Height / 2f;

            var linkAttributes = new NSMutableDictionary();
            linkAttributes.Add(UIStringAttributeKey.UnderlineStyle, NSNumber.FromBoolean(true));

            Body.LinkAttributes = new NSDictionary();
            Body.ActiveLinkAttributes = linkAttributes;

            Header.LinkAttributes = new NSDictionary();
            Header.ActiveLinkAttributes = linkAttributes;

            DefaultContentConstraint = ContentConstraint.Constant;

            ActionImage.TintColor = Time.TextColor;

            this.WhenAnyValue(x => x.ViewModel)
                .Where(x => x != null)
                .Subscribe(x =>
                {
                    var header = CreateAttributedStringFromBlocks(UIFont.PreferredBody, Theme.CurrentTheme.MainTextColor, x.Header);
                    var body = CreateAttributedStringFromBlocks(UIFont.PreferredSubheadline, Theme.CurrentTheme.MainSubtitleColor, x.Body);
                    Set(x.Avatar, x.CreatedOn, ChooseImage(x.EventType), header.Item1, body.Item1, header.Item2, body.Item2, x.Multilined);
                });
        }

        private static Tuple<NSMutableAttributedString, List<Link>> CreateAttributedStringFromBlocks(UIFont font, UIColor primaryColor, IEnumerable<EventTextBlock> blocks)
        {
            var attributedString = new NSMutableAttributedString();
            var links = new List<Link>();

            nint lengthCounter = 0;
            int i = 0;

            foreach (var b in blocks)
            {
                var tapped = (b as EventAnchorBlock)?.Tapped;

                UIColor color = null;
                if (tapped != null)
                    color = LinkColor;

                color = color ?? primaryColor;

                var ctFont = new CoreText.CTFont(font.Name, font.PointSize);
                var str = new NSAttributedString(b.Text, new CoreText.CTStringAttributes() { ForegroundColor = color.CGColor, Font = ctFont });
                attributedString.Append(str);
                var strLength = str.Length;

                if (tapped != null)
                {
                    var weakTapped = new WeakReference<Action>(tapped);
                    links.Add(new Link { Range = new NSRange(lengthCounter, strLength), Callback = () => weakTapped.Get()?.Invoke(), Id = i++ });
                }

                lengthCounter += strLength;
            }

            return new Tuple<NSMutableAttributedString, List<Link>>(attributedString, links);
        }

        class LabelDelegate : Xamarin.TTTAttributedLabel.TTTAttributedLabelDelegate {

            private readonly List<Link> _links;

            public LabelDelegate(List<Link> links)
            {
                _links = links;
            }

            public override void DidSelectLinkWithURL (Xamarin.TTTAttributedLabel.TTTAttributedLabel label, NSUrl url)
            {
                try
                {
                    if (!url.ToString().StartsWith("http", StringComparison.Ordinal))
                    {
                        var i = int.Parse(url.ToString());
                        _links[i].Callback();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to callback on TTTAttributedLabel: {0}", e.Message);
                }
            }
        }

        public void Set(Avatar avatar, string time, AtlassianIcon actionImage, 
            NSMutableAttributedString header, NSMutableAttributedString body, 
            List<Link> headerLinks, List<Link> bodyLinks, bool multilined)
        {
            Time.Text = time;
            ActionImage.Image = actionImage.ToImage();
            Body.Hidden = body.Length == 0;
            Body.Lines = multilined ? 0 : 4;
            ContentConstraint.Constant = Body.Hidden ? 0 : DefaultContentConstraint;
            Image.SetAvatar(avatar);

            if (header == null)
                header = new NSMutableAttributedString();
            if (body == null)
                body = new NSMutableAttributedString();

            Header.AttributedText = header;
            Header.Delegate = new LabelDelegate(headerLinks);

            Body.AttributedText = body;
            Body.Hidden = body.Length == 0;
            Body.Delegate = new LabelDelegate(bodyLinks);

            foreach (var b in headerLinks)
                Header.AddLinkToURL(new NSUrl(b.Id.ToString()), b.Range);

            foreach (var b in bodyLinks)
                Body.AddLinkToURL(new NSUrl(b.Id.ToString()), b.Range);
        }


        private static AtlassianIcon ChooseImage(string eventName)
        {
            switch (eventName)
            {
                case EventItem.Type.ForkRepo:
                    return AtlassianIcon.Devtoolsrepositoryforked;
                case EventItem.Type.CreateRepo:
                    return AtlassianIcon.Devtoolsrepository;
                case EventItem.Type.Commit:
                case EventItem.Type.Pushed:
                case EventItem.Type.PullRequestFulfilled:
                    return AtlassianIcon.Devtoolscommit;
                case EventItem.Type.WikiUpdated:
                case EventItem.Type.WikiCreated:
                case EventItem.Type.PullRequestUpdated:
                    return AtlassianIcon.Edit;
                case EventItem.Type.WikiDeleted:
                case EventItem.Type.DeleteRepo:
                    return AtlassianIcon.Delete;
                case EventItem.Type.StartFollowUser:
                case EventItem.Type.StartFollowRepo:
                case EventItem.Type.StopFollowRepo:
                case EventItem.Type.StartFollowIssue:
                case EventItem.Type.StopFollowIssue:
                    return AtlassianIcon.Star;
                case EventItem.Type.IssueComment:
                case EventItem.Type.ChangeSetCommentCreated:
                case EventItem.Type.ChangeSetCommentDeleted:
                case EventItem.Type.ChangeSetCommentUpdated:
                case EventItem.Type.PullRequestCommentCreated:
                case EventItem.Type.PullRequestCommentUpdated:
                case EventItem.Type.PullRequestCommentDeleted:
                    return AtlassianIcon.Comment;
                case EventItem.Type.IssueUpdated:
                case EventItem.Type.IssueReported:
                    return AtlassianIcon.Flag;
                case EventItem.Type.ChangeSetLike:
                case EventItem.Type.PullRequestLike:
                    return AtlassianIcon.Like;
                case EventItem.Type.PullRequestUnlike:
                case EventItem.Type.PullRequestRejected:
                case EventItem.Type.ChangeSetUnlike:
                    return AtlassianIcon.Like;
                case EventItem.Type.PullRequestCreated:
                case EventItem.Type.PullRequestSuperseded:
                    return AtlassianIcon.Devtoolspullrequest;
                default:
                    return AtlassianIcon.Info;
            }
        }
    }
}

