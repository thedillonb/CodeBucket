using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;

namespace CodeFramework.UI.Views
{
    public class SearchFilterBar : UISearchBar
    {
        public static UIImage ButtonBackground;
        public static UIImage FilterImage;
        private UIButton _button;

        public UIButton FilterButton { get { return _button; } }

        public bool FilterButtonVisible
        {
            get
            {
                //return !_button.Hidden;
                return false;
            }
            set
            {
                //_button.Hidden = !value;
            }
        }

        public SearchFilterBar()
            : base(new RectangleF(0, 0, 320, 44f))
        {
            /*
            _button = new UIButton(UIButtonType.Custom);
            _button.SetBackgroundImage(ButtonBackground, UIControlState.Normal);
            _button.SetImage(FilterImage, UIControlState.Normal);
            _button.SizeToFit();
            this.AddSubview(_button);
            */
        }

        /*
        public override void LayoutSubviews()
        {
            this.AutosizesSubviews = true;
            var bounds = this.Bounds;
            base.LayoutSubviews();

            if (FilterButtonVisible)
            {
                var buttonWidth = 44f;
                _button.Frame = new RectangleF(bounds.Width - 5 - buttonWidth, 7f, buttonWidth, 30f);

                var searchBar = this.Subviews.GetValue(1) as UIView;
                var frame = searchBar.Frame;
                frame.Width -= (_button.Frame.Width + 10f);
                searchBar.Frame = frame;
            }
        }
        */
    }

}

