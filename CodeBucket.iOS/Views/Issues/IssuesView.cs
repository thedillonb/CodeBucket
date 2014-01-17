using System;
using CodeBucket.Core.ViewModels.Issues;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using CodeFramework.ViewControllers;
using CodeFramework.Elements;

namespace CodeBucket.iOS.Views.Issues
{
	public class IssuesView : ViewModelCollectionDrivenDialogViewController
    {
        private UISegmentedControl _viewSegment;
        private UIBarButtonItem _segmentBarButton;

        public new IssuesViewModel ViewModel
        {
            get { return (IssuesViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

		public IssuesView()
		{
			Root.UnevenRows = true;
			Title = "Issues".t();
		}

		protected MonoTouch.Dialog.Element CreateElement(IssueModel x)
		{
			var assigned = x.Responsible != null ? x.Responsible.Username : "unassigned";
			var kind = x.Metadata.Kind;
			var commentString = x.CommentCount == 1 ? "1 comment".t() : x.CommentCount + " comments".t();
			var el = new IssueElement(x.LocalId.ToString(), x.Title, assigned, x.Status, commentString, kind, x.UtcLastUpdated);
			el.Tag = x;

			el.Tapped += () => {
				//Make sure the first responder is gone.
				View.EndEditing(true);
				ViewModel.GoToIssueCommand.Execute(x);
			};

			return el;
		}

        public override void ViewDidLoad()
        {
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => ViewModel.GoToNewIssueCommand.Execute(null));

            base.ViewDidLoad();

            _viewSegment = new UISegmentedControl(new string[] { "Open".t(), "Closed".t(), "Mine".t(), "Custom".t() });
			_viewSegment.ValueChanged += (s, e) => ViewModel.SelectedView = _viewSegment.SelectedSegment;
            _segmentBarButton = new UIBarButtonItem(_viewSegment);
            _segmentBarButton.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
            BindCollection(ViewModel.Issues, CreateElement);
        }

        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);
            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

