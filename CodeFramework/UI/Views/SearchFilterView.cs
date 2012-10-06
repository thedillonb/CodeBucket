using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;

namespace CodeFramework.UI.Views
{
    public class SearchFilterView : UIToolbar
    {
        public SearchFilterView()
            : base(new RectangleF(0, 0, 320, 44f))
        {
            var searchBar = new UISearchBar();
            var frame = searchBar.Frame;
            frame.Width = 100f;
            searchBar.Frame = frame;
            var filterButton = new UIBarButtonItem(UIBarButtonSystemItem.Organize, (s, e) => { });
            filterButton.Style = UIBarButtonItemStyle.Bordered;

            this.Items = new UIBarButtonItem[] { new UIBarButtonItem(searchBar) { Width = 0 }, filterButton };
        }
    }

    public class SearchFilterBar : UISearchBar
    {
        public static UIImage ButtonBackground;
        public static UIImage FilterImage;
        private UIButton _button;

        public bool FilterButtonVisible
        {
            get
            {
                return !_button.Hidden;
            }
            set
            {
                _button.Hidden = !value;
            }
        }

        public SearchFilterBar()
            : base(new RectangleF(0, 0, 320, 44f))
        {
            _button = new UIButton(UIButtonType.Custom);
            _button.SetBackgroundImage(ButtonBackground, UIControlState.Normal);
            _button.SetImage(FilterImage, UIControlState.Normal);
            _button.SizeToFit();
            this.AddSubview(_button);
            
        }

        public override void LayoutSubviews()
        {
            this.AutosizesSubviews = true;
            var bounds = this.Bounds;
            base.LayoutSubviews();

            if (FilterButtonVisible)
            {
                var buttonWidth = 50f;
                _button.Frame = new RectangleF(bounds.Width - 5 - buttonWidth, 7f, buttonWidth, 30f);

                var searchBar = this.Subviews.GetValue(1) as UIView;
                var frame = searchBar.Frame;
                frame.Width -= (_button.Frame.Width + 10f);
                searchBar.Frame = frame;
            }
        }
    }

}

