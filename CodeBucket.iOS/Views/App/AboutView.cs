using System;
using MonoTouch.Dialog;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using CodeFramework.iOS.ViewControllers;

namespace CodeBucket.iOS.Views.App
{
	public class AboutView : ViewModelDrivenViewController
    {
		private const string About = "CodeBucket is the best way to browse and maintain your Bitbucket repositories on any iOS device! " +
		                             "Keep an eye on your projects with the ability to view everything from followers to the individual file diffs in the latest change set. " +
		                             "CodeBucket brings Bitbucket to your finger tips in a sleek and efficient design. " + 
		                             "\n\nCreated By Dillon Buchanan";

        public AboutView()
            : base (true)
        {
            Title = "About".t();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var root = new RootElement(Title)
            {
                new Section
                {
                    new MultilinedElement("CodeBucket".t()) { Value = About, CaptionColor = Theme.CurrentTheme.MainTitleColor, ValueColor = Theme.CurrentTheme.MainTextColor }
                },
                new Section
                {
                    new StyledStringElement("Source Code".t(), () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://github.com/thedillonb/CodeBucket")))
                },
                new Section(String.Empty, "Thank you for downloading. Enjoy!")
                {
					new StyledStringElement("Follow On Twitter".t(), () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://twitter.com/Codebucketapp"))),
					new StyledStringElement("Rate This App".t(), () => UIApplication.SharedApplication.OpenUrl(new NSUrl("https://itunes.apple.com/us/app/codebucket/id551531422?mt=8"))),
                    new StyledStringElement("App Version".t(), NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleVersion")).ToString())
                }
            };

            root.UnevenRows = true;
            Root = root;
        }
    }
}

