using UIKit;

namespace CodeBucket
{
    public static class Images
    {
        public static UIImage RepoPlaceholder { get { return UIImage.FromBundle("Images/repository_placeholder"); } }
        public static UIImage Avatar { get { return UIImage.FromBundle("Images/avatar"); } }

        public static class Logos
        {
			public static UIImage Bitbucket { get { return UIImage.FromFile("Images/Logos/bitbucket.png"); } }
        }

        public static class Web
        {
            public static UIImage Back { get { return UIImageHelper.FromFileAuto("Images/Web/back"); } }
            public static UIImage Forward { get { return UIImageHelper.FromFileAuto("Images/Web/forward"); } }
        }

        public static class Buttons
        {
            public static UIImage Grey { get { return UIImageHelper.FromFileAuto("Images/Buttons/grey_button"); } }
            public static UIImage Check { get { return UIImageHelper.FromFileAuto("Images/Buttons/check"); } }
            public static UIImage Back { get { return UIImageHelper.FromFileAuto("Images/Buttons/back"); } }
            public static UIImage ThreeLines { get { return UIImageHelper.FromFileAuto("Images/Buttons/three_lines"); } }
            public static UIImage Cancel { get { return UIImageHelper.FromFileAuto("Images/Buttons/cancel"); } }
            public static UIImage Sort { get { return UIImageHelper.FromFileAuto("Images/Buttons/sort"); } }
            public static UIImage View { get { return UIImageHelper.FromFileAuto("Images/Buttons/view"); } }
            public static UIImage Fork { get { return UIImageHelper.FromFileAuto("Images/Buttons/fork"); } }
        }
    }
}

