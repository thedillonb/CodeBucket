using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using Cirrious.MvvmCross.Binding.BindingContext;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;
using CodeBucket.Core.Utils;
using CodeBucket.Cells;

namespace CodeBucket.Views.PullRequests
{
    public class PullRequestsView : ViewModelCollectionDrivenDialogViewController
    {
		private readonly UISegmentedControl _viewSegment;
		private readonly UIBarButtonItem _segmentBarButton;
 
		public PullRequestsView()
		{
			Root.UnevenRows = true;
			Title = "Pull Requests";
			NoItemsText = "No Pull Requests";

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

            BindCollection(vm.PullRequests, s => {
                var avatar = new Avatar(s.Author?.Links?.Avatar?.Href);
                var sse = new PullRequestElement(s.Title, s.UpdatedOn, avatar, Images.Avatar);
				sse.Tapped += () => vm.GoToPullRequestCommand.Execute(s);
                return sse;
            });
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

