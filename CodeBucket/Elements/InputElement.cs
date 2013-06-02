using MonoTouch.Dialog;
using MonoTouch.UIKit;

namespace CodeBucket.Elements
{
    public class InputElement : EntryElement
    {
        public InputElement(string caption, string placeholder, string value)
            : base(caption, placeholder, value)
        {
            TitleFont = StyledElement.DefaultTitleFont;
            EntryFont = UIFont.SystemFontOfSize(14f);
            TitleColor = StyledElement.DefaultTitleColor;
        }
    }
}

