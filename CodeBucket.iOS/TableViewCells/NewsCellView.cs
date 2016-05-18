using System;
using Foundation;
using UIKit;
using System.Collections.Generic;
using CodeBucket.Core.ViewModels.Events;
using ReactiveUI;
using System.Reactive.Linq;
using BitbucketSharp.Models;
using CodeBucket.Core.Utils;
using System.Linq;

namespace CodeBucket.TableViewCells
{
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

            Body.LinkAttributes = new NSDictionary();
            Body.ActiveLinkAttributes = new NSMutableDictionary();
            Body.ActiveLinkAttributes[CoreText.CTStringAttributeKey.UnderlineStyle] = NSNumber.FromBoolean(true);

            Header.LinkAttributes = new NSDictionary();
            Header.ActiveLinkAttributes = new NSMutableDictionary();
            Header.ActiveLinkAttributes[CoreText.CTStringAttributeKey.UnderlineStyle] = NSNumber.FromBoolean(true);

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

        class LabelDelegate : MonoTouch.TTTAttributedLabel.TTTAttributedLabelDelegate {

            private readonly List<Link> _links;

            public LabelDelegate(List<Link> links)
            {
                _links = links;
            }

            public override void DidSelectLinkWithURL (MonoTouch.TTTAttributedLabel.TTTAttributedLabel label, NSUrl url)
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

        public void Set(Avatar avatar, string time, UIImage actionImage, 
            NSMutableAttributedString header, NSMutableAttributedString body, 
            List<Link> headerLinks, List<Link> bodyLinks, bool multilined)
        {
            Time.Text = time;
            ActionImage.Image = actionImage;
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
                case EventModel.Type.ForkRepo:
                    return AtlassianIcon.Devtoolsrepositoryforked;
                case EventModel.Type.CreateRepo:
                    return AtlassianIcon.Devtoolsrepository;
                case EventModel.Type.Commit:
                case EventModel.Type.Pushed:
                case EventModel.Type.PullRequestFulfilled:
                    return AtlassianIcon.Devtoolscommit;
                case EventModel.Type.WikiUpdated:
                case EventModel.Type.WikiCreated:
                case EventModel.Type.PullRequestUpdated:
                    return AtlassianIcon.Edit;
                case EventModel.Type.WikiDeleted:
                case EventModel.Type.DeleteRepo:
                    return AtlassianIcon.Delete;
                case EventModel.Type.StartFollowUser:
                case EventModel.Type.StartFollowRepo:
                case EventModel.Type.StopFollowRepo:
                case EventModel.Type.StartFollowIssue:
                case EventModel.Type.StopFollowIssue:
                    return AtlassianIcon.Star;
                case EventModel.Type.IssueComment:
                case EventModel.Type.ChangeSetCommentCreated:
                case EventModel.Type.ChangeSetCommentDeleted:
                case EventModel.Type.ChangeSetCommentUpdated:
                case EventModel.Type.PullRequestCommentCreated:
                case EventModel.Type.PullRequestCommentUpdated:
                case EventModel.Type.PullRequestCommentDeleted:
                    return AtlassianIcon.Comment;
                case EventModel.Type.IssueUpdated:
                case EventModel.Type.IssueReported:
                    return AtlassianIcon.Flag;
                case EventModel.Type.ChangeSetLike:
                case EventModel.Type.PullRequestLike:
                    return AtlassianIcon.Like;
                case EventModel.Type.PullRequestUnlike:
                case EventModel.Type.PullRequestRejected:
                case EventModel.Type.ChangeSetUnlike:
                    return AtlassianIcon.Like;
                case EventModel.Type.PullRequestCreated:
                case EventModel.Type.PullRequestSuperseded:
                    return AtlassianIcon.Devtoolspullrequest;
                default:
                    return AtlassianIcon.Info;
            }
        }
    }
}

