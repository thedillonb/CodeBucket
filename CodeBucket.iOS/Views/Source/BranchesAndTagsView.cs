using CodeBucket.Core.ViewModels.Source;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using System;

namespace CodeBucket.Views.Source
{
	public class BranchesAndTagsView : ViewModelCollectionDrivenDialogViewController
	{
        public BranchesAndTagsView()
        {
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Filecode.ToEmptyListImage(), "There are no items."));
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var viewSegment = new UISegmentedControl(new object[] {"Branches", "Tags"});
            NavigationItem.TitleView = viewSegment;

			var vm = (BranchesAndTagsViewModel)ViewModel;
            var weakVm = new WeakReference<BranchesAndTagsViewModel>(vm);

            BindCollection(vm.Items, x => {
                var e = new StringElement(x.Name);
                e.Clicked.Subscribe(MakeCallback(weakVm, x));
                return e;
            });
		
            OnActivation(d =>
            {
                d(vm.Bind(x => x.SelectedFilter, true).Subscribe(x => viewSegment.SelectedSegment = x));
                d(viewSegment.GetChangedObservable().Subscribe(x => vm.SelectedFilter = x));
            });
		}

        private static Action<object> MakeCallback(WeakReference<BranchesAndTagsViewModel> weakVm, object model)
        {
            return new Action<object>(_ => weakVm.Get()?.GoToSourceCommand.Execute(model));
        }
	}
}

