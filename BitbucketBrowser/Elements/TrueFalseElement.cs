using MonoTouch.Dialog;

namespace CodeBucket.Elements
{
    public class TrueFalseElement : BooleanElement
    {
        public TrueFalseElement(string caption, bool value)
            : base(caption, value)
        {
        }

        public override MonoTouch.UIKit.UITableViewCell GetCell(MonoTouch.UIKit.UITableView tv)
        {
            var cell = base.GetCell(tv);
            cell.BackgroundColor = StyledElement.BgColor;
            cell.TextLabel.Font = StyledElement.DefaultTitleFont;
            cell.TextLabel.TextColor = StyledElement.DefaultTitleColor;
            return cell;
        }
    }
}

