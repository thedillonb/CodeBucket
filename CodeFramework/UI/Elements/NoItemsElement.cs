using System;
using MonoTouch.UIKit;

namespace CodeFramework.UI.Elements
{
    public class NoItemsElement : StyledElement
    {
        public NoItemsElement()
            : base("No Items")
        {
        }

        public override UITableViewCell GetCell(UITableView tv)
        {
            var c = base.GetCell(tv);
            c.TextLabel.TextAlignment = UITextAlignment.Center;
            return c;
        }

        public override string Summary()
        {
            return string.Empty;
        }
    }
}

