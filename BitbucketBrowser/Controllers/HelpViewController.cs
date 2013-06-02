namespace CodeBucket.Controllers
{
    public class HelpViewController : WebViewController
    {
        public static string SupportUrl = "http://support.codebucket.dillonbuchanan.com";
        public HelpViewController()
        {
            Title = "Help";
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            Web.LoadRequest(new MonoTouch.Foundation.NSUrlRequest(new MonoTouch.Foundation.NSUrl(SupportUrl)));
        }
    }
}

