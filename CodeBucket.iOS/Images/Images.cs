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

        public static class Buttons
        {
            public static UIImage GreyButton { get { return UIImageHelper.FromFileAuto("Images/Buttons/grey_button"); } }
        }
    }
}

