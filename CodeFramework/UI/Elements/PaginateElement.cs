using System;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using CodeFramework.UI.Views;

namespace CodeFramework.UI.Elements
{
    public class PaginateElement : LoadMoreElement
    {
        static PaginateElement()
        {
            LoadMoreElement.Padding = 20;
        }

        public PaginateElement(string normal, string loading, Action<LoadMoreElement> tap)
            : base(normal, loading, tap)
        {
            Font = StyledElement.TitleFont;
            this.TextColor = UIColor.FromRGB(41, 41, 41);
        }

        protected override void CellCreated(UITableViewCell cell, UIView view)
        {
            base.CellCreated(cell, view);
            cell.BackgroundView = new CellBackgroundView();
        }
    }
}

