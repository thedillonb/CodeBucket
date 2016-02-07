using UIKit;
using CodeBucket.Elements;

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
			Theme.CurrentTheme = theme;
			StyledStringElement.DefaultTitleFont = UIFont.SystemFontOfSize(15f);

            UIApplication.SharedApplication.StatusBarStyle = UIStatusBarStyle.LightContent;

            var backgroundImg = CreateBackgroundImage(Theme.CurrentTheme.PrimaryColor);

            UINavigationBar.Appearance.TintColor = UIColor.White;
            UINavigationBar.Appearance.BarTintColor = Theme.CurrentTheme.PrimaryColor;
            UINavigationBar.Appearance.BackgroundColor = Theme.CurrentTheme.PrimaryColor;
            UINavigationBar.Appearance.SetBackgroundImage(backgroundImg, UIBarMetrics.Default);
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes { TextColor = UIColor.White, Font = UIFont.SystemFontOfSize(18f) });
            UINavigationBar.Appearance.BackIndicatorImage = Theme.CurrentTheme.BackButton;
            UINavigationBar.Appearance.BackIndicatorTransitionMaskImage = Theme.CurrentTheme.BackButton;

            UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(0, -64), UIBarMetrics.LandscapePhone);
            UIBarButtonItem.Appearance.SetBackButtonTitlePositionAdjustment(new UIOffset(0, -64), UIBarMetrics.Default);


            CodeBucket.Utils.Hud.BackgroundTint = UIColor.FromRGBA(228, 228, 228, 128);

            UISegmentedControl.Appearance.TintColor = Theme.CurrentTheme.PrimaryColor;
            UITableViewHeaderFooterView.Appearance.TintColor = UIColor.FromRGB(228, 228, 228);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).TextColor = UIColor.FromRGB(136, 136, 136);
            UILabel.AppearanceWhenContainedIn(typeof(UITableViewHeaderFooterView)).Font = UIFont.SystemFontOfSize(13f);

            UIToolbar.Appearance.BarTintColor = UIColor.FromRGB(245, 245, 245);

            UIBarButtonItem.AppearanceWhenContainedIn(typeof(UISearchBar)).SetTitleTextAttributes(new UITextAttributes()
            {
                TextColor = UIColor.White,
            }, UIControlState.Normal);

            CodeBucket.Views.StartupView.TextColor = UIColor.FromWhiteAlpha(0.9f, 1.0f);
            CodeBucket.Views.StartupView.SpinnerColor = UIColor.FromWhiteAlpha(0.85f, 1.0f);

            UIImageView.AppearanceWhenContainedIn(typeof(UITableViewCell), typeof(MonoTouch.SlideoutNavigation.MainNavigationController)).TintColor = Theme.CurrentTheme.IconColor;

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
		public UIImage CheckButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/check"); } }
		public UIImage BackButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/back"); } }
		public UIImage ThreeLinesButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/three_lines"); } }
		public UIImage CancelButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/cancel"); } }
		public UIImage SortButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/sort"); } }
		public UIImage SaveButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/save"); } }
		public UIImage ViewButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/view"); } }
		public UIImage ForkButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/fork"); } }
		public UIImage WebBackButton { get { return UIImageHelper.FromFileAuto("Images/Web/back"); } }
		public UIImage WebFowardButton { get { return UIImageHelper.FromFileAuto("Images/Web/forward"); } }

		public UIColor ViewBackgroundColor { get { return UIColor.FromRGB(238, 238, 238); } }

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

		public UIColor AccountsNavigationBarTint
		{
			get
			{
				return UIColor.Red;
			}
		}

		public UIColor SlideoutNavigationBarTint
		{
			get
			{
				return UIColor.Black;
			}
		}

		public UIColor ApplicationNavigationBarTint
		{
			get
			{
				return UIColor.Black;
			}
		}

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