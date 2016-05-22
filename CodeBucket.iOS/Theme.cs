using UIKit;
using CodeBucket.Views;
using CodeBucket.ViewControllers;
using MonoTouch.SlideoutNavigation;
using System.Linq;

namespace CodeBucket
{
    public class Theme
	{
		public static Theme CurrentTheme { get; private set; }

        private static UIImage CreateBackgroundImage(UIColor color)
        {
            UIGraphics.BeginImageContext(new CoreGraphics.CGSize(1, 1f));
            color.SetFill();
            UIGraphics.RectFill(new CoreGraphics.CGRect(0, 0, 1, 1));
            var img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return img;
        }

		public static void Setup()
		{
			var theme = new Theme();
			CurrentTheme = theme;

            var backgroundImg = CreateBackgroundImage(Theme.CurrentTheme.PrimaryColor);
            var barTypes = new [] { typeof(MainNavigationController), typeof(ThemedNavigationController) };
            foreach (var bar in barTypes.Select(x => UINavigationBar.AppearanceWhenContainedIn(x)))
                SetApperance(bar, backgroundImg, Theme.CurrentTheme.PrimaryColor);

            var menuColor = UIColor.FromRGB(50, 50, 50);
            var menuImg = CreateBackgroundImage(menuColor);
            var menuApp = UINavigationBar.AppearanceWhenContainedIn(typeof(MenuNavigationController));
            SetApperance(menuApp, menuImg, menuColor);

            foreach (var buttonItem in barTypes.Select(x => UIBarButtonItem.AppearanceWhenContainedIn(x)))
            {
                buttonItem.SetBackButtonTitlePositionAdjustment(new UIOffset(0, -float.MaxValue), UIBarMetrics.LandscapePhone);
                buttonItem.SetBackButtonTitlePositionAdjustment(new UIOffset(0, -float.MaxValue), UIBarMetrics.Default);
            }

            UISegmentedControl.AppearanceWhenContainedIn(typeof(UIToolbar)).TintColor = Theme.CurrentTheme.PrimaryColor;

            UITableViewHeaderFooterView.Appearance.TintColor = UIColor.FromRGB(228, 228, 228);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).TextColor = UIColor.FromRGB(136, 136, 136);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).Font = UIFont.SystemFontOfSize(13f);

            UIToolbar.Appearance.BarTintColor = UIColor.FromRGB(245, 245, 245);

            UIBarButtonItem.AppearanceWhenContainedIn(typeof(UISearchBar)).SetTitleTextAttributes(new UITextAttributes {TextColor  = UIColor.White }, UIControlState.Normal);

            UIImageView.AppearanceWhenContainedIn(typeof(UITableViewCell), typeof(MainNavigationController)).TintColor = Theme.CurrentTheme.IconColor;

            EmptyListView.DefaultColor = Theme.CurrentTheme.PrimaryColor;
		}

        private static void SetApperance(UINavigationBar.UINavigationBarAppearance bar, UIImage backgroundImg, UIColor color)
        {
            bar.TintColor = UIColor.White;
            bar.BarTintColor = bar.BackgroundColor = color;
            bar.SetBackgroundImage(backgroundImg, UIBarMetrics.Default);
            bar.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(18f) });
            bar.BackIndicatorImage = Images.Buttons.Back;
            bar.BackIndicatorTransitionMaskImage = Images.Buttons.Back;
        }

		public UITextAttributes SegmentedControlText
		{
			get
			{
				return new UITextAttributes
				{ 
					Font = UIFont.SystemFontOfSize(14f), 
					TextColor = UIColor.FromRGB(87, 85, 85), 
					TextShadowColor = UIColor.FromRGBA(255, 255, 255, 125), 
					TextShadowOffset = new UIOffset(0, 1) 
				};
			}
		}

		public UIImage IssueCellImage1
		{
            get { return AtlassianIcon.Configure.ToImage(); }
		}

		public UIImage IssueCellImage2
		{
            get { return AtlassianIcon.Comment.ToImage(); }
		}

		public UIImage IssueCellImage3
		{
            get { return AtlassianIcon.User.ToImage(); }
		}

		public UIImage IssueCellImage4
		{
            get { return AtlassianIcon.Edit.ToImage(); }
		}

		public UIColor NavigationTextColor { get { return UIColor.FromRGB(97, 95, 95); } }

        public UIColor MainTitleColor { get { return SecondaryColor; } }
		public UIColor MainSubtitleColor { get { return UIColor.FromRGB(81, 81, 81); } }
		public UIColor MainTextColor { get { return UIColor.FromRGB(41, 41, 41); } }

		public UIColor IssueTitleColor { get { return MainTitleColor; } }
		public UIColor RepositoryTitleColor { get { return MainTitleColor; } }
		public UIColor HeaderViewTitleColor { get { return MainTitleColor; } }
		public UIColor HeaderViewDetailColor { get { return MainSubtitleColor; } }

		public UIColor WebButtonTint { get { return UIColor.FromRGB(127, 125, 125); } }

		public UIColor PrimaryNavigationBarTintColor
		{
			get { return UIColor.White; }
		}

		public UIColor PrimaryNavigationBarBarTintColor
		{
            get { return PrimaryColor; }
		}

        public UIColor PrimaryColor
        {
            get { return UIColor.FromRGB(0x20, 0x50, 0x81); } //UIColor.FromRGB(45, 80, 148)
        }

        public UIColor SecondaryColor
        {
            get { return UIColor.FromRGB(0x35, 0x72, 0xb0); }
            //            var iconColor = UIColor.FromRGB(0x70, 0x70, 0x70);#3572b0
        }


        public UIColor IconColor
        {
            get { return UIColor.FromRGB(0x70, 0x70, 0x70); }
        }

		public UITextAttributes PrimaryNavigationBarTextAttributes
		{
			get { return new UITextAttributes { TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(18f) }; }
		}
	}
}