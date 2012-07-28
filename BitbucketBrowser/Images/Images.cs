using System;
using MonoTouch.UIKit;

namespace BitbucketBrowser
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

        public static UIImage CellGradient = UIImage.FromBundle("/Images/Cells/gradient");

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
        public static UIImage BarButton = UIImage.FromBundle("/Images/Controls/barbutton");
        public static UIImage Titlebar = UIImage.FromBundle("/Images/Controls/titlebar");
        public static UIImage TitlebarDark = UIImage.FromBundle("/Images/Controls/titlebar_dark");
        public static UIImage Bottombar = UIImage.FromBundle("/Images/Controls/bottombar");
        public static UIImage Searchbar = UIImage.FromBundle("/Images/Controls/searchbar");
        public static UIImage Divider = UIImage.FromBundle("/Images/Controls/divider");
        public static UIImage Logo = UIImage.FromBundle("/Images/Controls/logo");

        public static UIImage Unknown = UIImage.FromBundle("/Images/unknown");

        //Issues
        public static UIImage Priority = UIImage.FromBundle("/Images/priority");

        public static UIImage Anonymous = UIImage.FromBundle("/Images/Cells/anonymous");


        //Size agnostic
        public static UIImage Background = UIImage.FromFile("Images/Controls/background.png");
        public static UIImage Linen = UIImage.FromFile("Images/linen.png");
    }
}

