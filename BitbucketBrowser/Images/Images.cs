using MonoTouch.UIKit;

namespace CodeBucket.Images
{
    public static class Images
    {
        public static UIImage ScmType = UIImage.FromBundle("/Images/scm_type");
        public static UIImage Language = UIImage.FromBundle("/Images/language");
        public static UIImage Webpage = UIImage.FromBundle("/Images/webpage");
        public static UIImage Repo = UIImage.FromBundle("/Images/repo");
        public static UIImage Size = UIImage.FromBundle("/Images/size");
        public static UIImage Locked = UIImage.FromBundle("/Images/locked");
        public static UIImage Unlocked = UIImage.FromBundle("/Images/unlocked");
        public static UIImage Heart = UIImage.FromBundle("/Images/heart");
        public static UIImage HeartAdd = UIImage.FromBundle("/Images/heart_add");
        public static UIImage HeartDelete = UIImage.FromBundle("/Images/heart_delete");
        public static UIImage Fork = UIImage.FromBundle("/Images/fork");
        public static UIImage Pencil = UIImage.FromBundle("/Images/pencil");
        public static UIImage Plus = UIImage.FromBundle("/Images/plus");
        public static UIImage Tag = UIImage.FromBundle("/Images/tag");
        public static UIImage CommentAdd = UIImage.FromBundle("/Images/comment_add");
        public static UIImage ReportEdit = UIImage.FromBundle("/Images/report_edit");
        public static UIImage BinClosed = UIImage.FromBundle("/Images/bin_closed");
        public static UIImage Milestone = UIImage.FromBundle("/Images/milestone");
        public static UIImage ServerComponents = UIImage.FromBundle("/Images/server_components");
        public static UIImage SitemapColor = UIImage.FromBundle("/Images/sitemap_color");

        public static UIImage CellGradient = UIImage.FromBundle("/Images/gradient");

        public static UIImage Flag = UIImage.FromBundle("/Images/flag");

        public static UIImage Folder = UIImage.FromBundle("/Images/folder");
        public static UIImage File = UIImage.FromBundle("/Images/file");
        public static UIImage Branch = UIImage.FromBundle("/Images/branch");
        public static UIImage Create = UIImage.FromBundle("/Images/create");

        public static UIImage Changes = UIImage.FromBundle("/Images/changes");
        public static UIImage ChangeUser = UIImage.FromBundle("/Images/change_user");

        //Tabs
        public static UIImage Group = UIImage.FromBundle("/Images/Tabs/group");
        public static UIImage Event = UIImage.FromBundle("/Images/Tabs/events");
        public static UIImage Person = UIImage.FromBundle("/Images/Tabs/person");
        public static UIImage Cog = UIImage.FromBundle("/Images/Tabs/cog");

        //Controls
        public static UIImage BackButton = UIImage.FromBundle("/Images/Controls/backbutton");
        public static UIImage BackButtonLandscape = UIImage.FromBundle("/Images/Controls/backbutton-landscape");

        public static UIImage BarButton = UIImage.FromBundle("/Images/Controls/barbutton");
        public static UIImage BarButtonLandscape = UIImage.FromBundle("/Images/Controls/barbutton-land");

        public static UIImage Titlebar = UIImage.FromBundle("/Images/Controls/titlebar");
        public static UIImage TitlebarDark = UIImage.FromBundle("/Images/Controls/titlebar_dark");
        public static UIImage Bottombar = UIImage.FromFile("Images/Controls/bottombar.png");
        public static UIImage Searchbar = UIImage.FromBundle("/Images/Controls/searchbar");
        public static UIImage Divider = UIImage.FromBundle("/Images/Controls/divider");

        public static UIImage TableCell = UIImage.FromBundle("/Images/TableCell");
        public static UIImage TableCellRed = UIImage.FromBundle("/Images/tablecell_red");

		public static UIImage BackNavigationButton = UIImage.FromFile("Images/back_button@2x.png");
		public static UIImage ForwardNavigationButton = UIImage.FromFile("Images/forward_button@2x.png");

        public static UIImage Logo { get { return UIImageHelper.FromFileAuto("Images/Controls/logo"); } }
        public static UIImage LogoBehind 
        { 
            get 
            { 
                if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
                {
                    return UIImageHelper.FromFileAuto(MonoTouch.Utilities.IsTall ? 
                        "Images/Controls/logobehind-568h" : "Images/Controls/logobehind");
                }
                return UIImageHelper.FromFileAuto("Images/Controls/logobehind-portrait");
            } 
        }

        //Issues
        public static UIImage Priority = UIImage.FromBundle("/Images/priority");
        public static UIImage Anonymous = UIImage.FromBundle("/Images/anonymous");


        //Size agnostic
        public static UIImage Background = UIImage.FromFile("Images/Controls/background.png");
        public static UIImage Linen = UIImage.FromFile("Images/linen.png");

        public static UIImage Filter = UIImage.FromFile("Images/filter_results.png");

        public static UIImage ThreeLines = UIImage.FromFile("Images/three_lines.png");

		public static UIImage BitbucketLogo = UIImage.FromFile("Images/Logos/logoBitBucketPNG.png");
		public static UIImage GitHubLogo = UIImage.FromFile("Images/Logos/GitHub-Logo.png");
		
    }
}

