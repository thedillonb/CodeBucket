using System;
using CodeBucket.Core.ViewModels.Issues;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using CodeFramework.ViewControllers;
using CodeFramework.Elements;
using Cirrious.MvvmCross.Binding.BindingContext;
using CodeBucket.Core.Services;
using Cirrious.CrossCore;
using CodeBucket.Core.Filters;
using CodeBucket.iOS.Views.Filters;

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

			_viewSegment = new UISegmentedControl(new string[] { "All".t(), "Open".t(), "Mine".t(), "Custom".t() });
			_segmentBarButton = new UIBarButtonItem(_viewSegment);
			_segmentBarButton.Width = View.Frame.Width - 10f;
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
            BindCollection(ViewModel.Issues, CreateElement);
        }


		void SegmentValueChanged (object sender, EventArgs e)
		{
			var application = Mvx.Resolve<IApplicationService>();

			if (_viewSegment.SelectedSegment == 0)
			{
				ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateAllFilter(), true);
			}
			else if (_viewSegment.SelectedSegment == 1)
			{
				ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateOpenFilter(), true);
			}
			else if (_viewSegment.SelectedSegment == 2)
			{
				ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateMineFilter(application.Account.Username), true);
			}
			else if (_viewSegment.SelectedSegment == 3)
			{
				ShowFilterController(new IssuesFilterViewController(ViewModel.Issues));
			}
		}


        public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);

			//Before we select which one, make sure we detach the event handler or silly things will happen
			_viewSegment.ValueChanged -= SegmentValueChanged;

			var application = Mvx.Resolve<IApplicationService>();

			//Select which one is currently selected
			if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateAllFilter()))
				_viewSegment.SelectedSegment = 0;
			else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateOpenFilter()))
				_viewSegment.SelectedSegment = 1;
			else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateMineFilter(application.Account.Username)))
				_viewSegment.SelectedSegment = 2;
			else
				_viewSegment.SelectedSegment = 3;

			_viewSegment.ValueChanged += SegmentValueChanged;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

