using CodeBucket.Core.ViewModels.Source;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Elements;
using MvvmCross.Binding.BindingContext;

namespace CodeBucket.Views.Source
{
	public class BranchesAndTagsView : ViewModelCollectionDrivenDialogViewController
	{
		private UISegmentedControl _viewSegment;
		private UIBarButtonItem _segmentBarButton;

		public override void ViewDidLoad()
		{
			Title = "Source";
			NoItemsText = "No Items";

			base.ViewDidLoad();

			_viewSegment = new UISegmentedControl(new object[] {"Branches", "Tags"});
			_segmentBarButton = new UIBarButtonItem(_viewSegment) { Width = View.Frame.Width - 10f };
			ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };

			var vm = (BranchesAndTagsViewModel)ViewModel;
			this.BindCollection(vm.Items, x => new StyledStringElement(x.Name, () => vm.GoToSourceCommand.Execute(x)));
			var set = this.CreateBindingSet<BranchesAndTagsView, BranchesAndTagsViewModel>();
			set.Bind(_viewSegment).To(x => x.SelectedFilter);
			set.Apply();
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

