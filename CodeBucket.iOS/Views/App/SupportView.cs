//using System;
//using CodeBucket.ViewControllers;
//using MvvmCross.Platform;
//using CodeBucket.Core.Services;
//using CodeBucket.Elements;
//using UIKit;
//
//namespace CodeBucket.Views.App
//{
//    public class SupportView : PrettyDialogViewController
//    {
//        public SupportView()
//        {
//            Title = "Feedback";
//        }
//
//        public override void ViewDidLoad()
//        {
//            base.ViewDidLoad();
//
//            var split = new SplitButtonElement();
//            var contributors = split.AddButton("Contributors", "-");
//            var lastCommit = split.AddButton("Last Commit", "-");
//
//            var addFeatureButton = new BigButtonElement("Suggest a feature", Octicon.LightBulb.ToImage());
//            var addBugButton = new BigButtonElement("Report a bug", Octicon.Bug.ToImage());
//            var featuresButton = new BigButtonElement("Submitted Work Items", Octicon.Clippy.ToImage());
//
//            HeaderView.SubText = "This app is the product of hard work and great suggestions! Thank you to all whom provide feedback!";
//            HeaderView.Image = UIImage.FromFile("Icon@2x.png");
//            RefreshHeaderView();
//
//            Root = new RootElement(Title) { new Section { split }, new Section { addFeatureButton, addBugButton }, new Section { featuresButton } };
//
//            OnActivation(d => {
//                d(addFeatureButton.Clicked.InvokeCommand(ViewModel.GoToSuggestFeatureCommand));
//                d(addBugButton.Clicked.InvokeCommand(ViewModel.GoToReportBugCommand));
//                d(featuresButton.Clicked.InvokeCommand(ViewModel.GoToFeedbackCommand));
//                d(HeaderView.Clicked.InvokeCommand(ViewModel.GoToRepositoryCommand));
//
//                d(this.WhenAnyValue(x => x.ViewModel.Contributors).Where(x => x.HasValue).SubscribeSafe(x =>
//                    contributors.Text = (x.Value >= 100 ? "100+" : x.Value.ToString())));
//
//                d(this.WhenAnyValue(x => x.ViewModel.LastCommit).Where(x => x.HasValue).SubscribeSafe(x =>
//                    lastCommit.Text = x.Value.UtcDateTime.Humanize()));
//            });
//        }
//
//        private class BigButtonElement : ButtonElement, IElementSizing
//        {
//            public BigButtonElement(string name, UIImage img)
//                : base(name, img)
//            {
//            }
//
//            public nfloat GetHeight(UITableView tableView, Foundation.NSIndexPath indexPath)
//            {
//                return 58f;
//            }
//        }
//    }
//}
//
