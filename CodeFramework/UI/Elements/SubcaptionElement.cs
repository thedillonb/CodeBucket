using MonoTouch.UIKit;

namespace CodeFramework.UI.Elements
{
    public class SubcaptionElement : StyledElement
    {
        private static readonly UIFont TitleFont = UIFont.BoldSystemFontOfSize(15f);
        private static readonly UIFont SubFont = UIFont.SystemFontOfSize(13f);

        public SubcaptionElement(string title, string subtitle)
            : base(title, subtitle, UITableViewCellStyle.Subtitle)
        {
            Font = TitleFont;
            SubtitleFont = SubFont;
        }

        public SubcaptionElement(string title)
            : this(title, null)
        {
        }
    }
}

