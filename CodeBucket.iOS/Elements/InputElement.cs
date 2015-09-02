using UIKit;

namespace CodeBucket.Elements
{
    public class InputElement : EntryElement
    {
        public InputElement(string caption, string placeholder, string value)
            : base(caption, placeholder, value)
        {
            TitleFont = StyledStringElement.DefaultTitleFont.WithSize(StyledStringElement.DefaultTitleFont.PointSize * Element.FontSizeRatio);
            EntryFont = UIFont.SystemFontOfSize(14 * Element.FontSizeRatio);
            TitleColor = StyledStringElement.DefaultTitleColor;
        }
    }
}

