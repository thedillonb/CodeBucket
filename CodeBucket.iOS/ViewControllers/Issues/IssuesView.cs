using System;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using BitbucketSharp.Models;
using CodeBucket.Core.Services;
using CodeBucket.Core.Filters;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;

namespace CodeBucket.Views.Issues
{
    public class IssuesView : ViewModelCollectionDrivenDialogViewController<IssuesViewModel>
    {
        private UISegmentedControl _viewSegment;
        private UIBarButtonItem _segmentBarButton;

		protected Element CreateElement(IssueModel x)
		{
			var assigned = x.Responsible != null ? x.Responsible.Username : "unassigned";
			var kind = x.Metadata.Kind;
			if (kind == "enhancement")
				kind = "enhance";

			var commentString = x.CommentCount == 1 ? "1 comment" : x.CommentCount + " comments";
			var el = new IssueElement(x.LocalId.ToString(), x.Title, assigned, x.Status, commentString, kind, x.UtcLastUpdated);

			//el.Tapped += () => {
			//	//Make sure the first responder is gone.
			//	View.EndEditing(true);
			//	ViewModel.GoToIssueCommand.Execute(x);
			//};

			return el;
		}

        public override void ViewDidLoad()
        {
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add, (s, e) => ViewModel.GoToNewIssueCommand.Execute(null));

            base.ViewDidLoad();

            _viewSegment = new CustomUISegmentedControl(new [] { "All", "Open", "Mine", "Custom" }, 3);
			_segmentBarButton = new UIBarButtonItem(_viewSegment);
			_segmentBarButton.Width = View.Frame.Width - 10f;
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
            //BindCollection(ViewModel.Issues, CreateElement);
        }


		//void SegmentValueChanged (object sender, EventArgs e)
		//{
		//	var application = Mvx.Resolve<IApplicationService>();

		//	if (_viewSegment.SelectedSegment == 0)
		//	{
		//		ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateAllFilter(), true);
		//	}
		//	else if (_viewSegment.SelectedSegment == 1)
		//	{
		//		ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateOpenFilter(), true);
		//	}
		//	else if (_viewSegment.SelectedSegment == 2)
		//	{
		//		ViewModel.Issues.ApplyFilter(IssuesFilterModel.CreateMineFilter(application.Account.Username), true);
		//	}
		//	else if (_viewSegment.SelectedSegment == 3)
		//	{
  //              ViewModel.GoToFiltersCommand.Execute(null);
		//	}
		//}


        public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, animated);

			//Before we select which one, make sure we detach the event handler or silly things will happen
			//_viewSegment.ValueChanged -= SegmentValueChanged;

			//var application = Mvx.Resolve<IApplicationService>();

			////Select which one is currently selected
			//if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateAllFilter()))
			//	_viewSegment.SelectedSegment = 0;
			//else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateOpenFilter()))
			//	_viewSegment.SelectedSegment = 1;
			//else if (ViewModel.Issues.Filter.Equals(IssuesFilterModel.CreateMineFilter(application.Account.Username)))
			//	_viewSegment.SelectedSegment = 2;
			//else
			//	_viewSegment.SelectedSegment = 3;

			//_viewSegment.ValueChanged += SegmentValueChanged;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }

        private class CustomUISegmentedControl : UISegmentedControl
        {
            readonly int _multipleTouchIndex;
            public CustomUISegmentedControl(object[] args, int multipleTouchIndex)
                : base(args)
            {
                this._multipleTouchIndex = multipleTouchIndex;
            }

            public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
            {
                var previousSelected = SelectedSegment;
                base.TouchesEnded(touches, evt);
                if (previousSelected == SelectedSegment && SelectedSegment == _multipleTouchIndex)
                    SendActionForControlEvents(UIControlEvent.ValueChanged);
            }
        }
    }
}

