using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using CodeBucket.TableViewCells;
using MvvmCross.Binding.BindingContext;
using System;

namespace CodeBucket.Views.PullRequests
{
    public class PullRequestsView : ViewModelCollectionDrivenDialogViewController
    {
		private readonly UISegmentedControl _viewSegment;
		private readonly UIBarButtonItem _segmentBarButton;
 
		public PullRequestsView()
		{
			Title = "Pull Requests";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolspullrequest.ToImage(64f), "There are no pull requests.")); 

			_viewSegment = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
			_segmentBarButton = new UIBarButtonItem(_viewSegment);
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(PullRequestCellView.Nib, PullRequestCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

			var vm = (PullRequestsViewModel)ViewModel;
            _segmentBarButton.Width = View.Frame.Width - 10f;
			var set = this.CreateBindingSet<PullRequestsView, PullRequestsViewModel>();
			set.Bind(_viewSegment).To(x => x.SelectedFilter);
			set.Apply();

            BindCollection(vm.PullRequests, s => new PullRequestElement(s, () => vm.GoToPullRequestCommand.Execute(s)));
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
			if (ToolbarItems != null)
				NavigationController.SetToolbarHidden(false, animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

