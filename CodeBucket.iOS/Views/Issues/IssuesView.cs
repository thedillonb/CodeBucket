//using System;
//using Cirrious.CrossCore;
//using CodeFramework.iOS.Views;
//using CodeBucket.Core.Filters;
//using CodeBucket.Core.Services;
//using CodeBucket.Core.ViewModels.Issues;
//using MonoTouch.UIKit;
//using CodeBucket.iOS.Views.Filters;
//using BitbucketSharp.Models;
//using CodeFramework.ViewControllers;
//
//namespace CodeBucket.iOS.Views.Issues
//{
//	public class IssuesView : ViewModelCollectionDrivenViewController
//    {
//        private UISegmentedControl _viewSegment;
//        private UIBarButtonItem _segmentBarButton;
//
//        public new IssuesViewModel ViewModel
//        {
//            get { return (IssuesViewModel)base.ViewModel; }
//            set { base.ViewModel = value; }
//        }
//
//		protected IssuesView()
//		{
//			Root.UnevenRows = true;
//			Title = "Issues".t();
//		}
//
//		protected MonoTouch.Dialog.Element CreateElement(IssueModel x)
//		{
//			var isPullRequest = x.PullRequest != null && !(string.IsNullOrEmpty(x.PullRequest.HtmlUrl));
//			var assigned = x.Assignee != null ? x.Assignee.Login : "unassigned";
//			var kind = isPullRequest ? "Pull" : "Issue";
//			var commentString = x.Comments == 1 ? "1 comment".t() : x.Comments + " comments".t();
//			var el = new IssueElement(x.Number.ToString(), x.Title, assigned, x.State, commentString, kind, x.UpdatedAt);
//			el.Tag = x;
//
//			el.Tapped += () => {
//				//Make sure the first responder is gone.
//				View.EndEditing(true);
//				ViewModel.GoToIssueCommand.Execute(x);
//			};
//
//			return el;
//		}
//
//        public override void ViewDidLoad()
//        {
//			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => ViewModel.GoToNewIssueCommand.Execute(null));
//
//            base.ViewDidLoad();
//
//            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t(), "Mine".t(), "Custom".t() });
//            _segmentBarButton = new UIBarButtonItem(_viewSegment);
//            _segmentBarButton.Width = View.Frame.Width - 10f;
//            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
//            BindCollection(ViewModel.Issues, CreateElement);
//        }
//
//        public override void ViewWillAppear(bool animated)
//        {
//            if (ToolbarItems != null)
//                NavigationController.SetToolbarHidden(false, animated);
//            base.ViewWillAppear(animated);
//
//            //Before we select which one, make sure we detach the event handler or silly things will happen
//            _viewSegment.ValueChanged -= SegmentValueChanged;
//
//            var application = Mvx.Resolve<IApplicationService>();
//
//            //Select which one is currently selected
//            if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateOpenFilter()))
//                _viewSegment.SelectedSegment = 0;
//            else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateClosedFilter()))
//                _viewSegment.SelectedSegment = 1;
//            else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateMineFilter(application.Account.Username)))
//                _viewSegment.SelectedSegment = 2;
//            else
//                _viewSegment.SelectedSegment = 3;
//
//            _viewSegment.ValueChanged += SegmentValueChanged;
//        }
//
//        void SegmentValueChanged (object sender, EventArgs e)
//        {
//            var application = Mvx.Resolve<IApplicationService>();
//
//            if (_viewSegment.SelectedSegment == 0)
//            {
//                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateOpenFilter(), true);
//            }
//            else if (_viewSegment.SelectedSegment == 1)
//            {
//                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateClosedFilter(), true);
//            }
//            else if (_viewSegment.SelectedSegment == 2)
//            {
//                ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateMineFilter(application.Account.Username), true);
//            }
//            else if (_viewSegment.SelectedSegment == 3)
//            {
//				//ShowFilterController(new IssuesFilterViewController(ViewModel.Username, ViewModel.Repository, ViewModel.Issues));
//            }
//        }
//
//        public override void ViewWillDisappear(bool animated)
//        {
//            base.ViewWillDisappear(animated);
//            if (ToolbarItems != null)
//                NavigationController.SetToolbarHidden(true, animated);
//        }
//    }
//}
//
