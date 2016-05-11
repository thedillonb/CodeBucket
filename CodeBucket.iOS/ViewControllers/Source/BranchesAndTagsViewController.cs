using CodeBucket.Core.ViewModels.Source;
using UIKit;
using CodeBucket.DialogElements;
using System;
using CodeBucket.Views;
using ReactiveUI;
using System.Linq;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesAndTagsViewController : ViewModelCollectionDrivenDialogViewController<BranchesAndTagsViewModel>
	{
        public BranchesAndTagsViewController()
        {
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Filecode.ToEmptyListImage(), "There are no items."));
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var viewSegment = new UISegmentedControl(new object[] {"Branches", "Tags"});
            NavigationItem.TitleView = viewSegment;

            ViewModel
                .Items
                .ChangedObservable()
                .Subscribe(x =>
                {
                    var section = new Section();
                    section.AddAll(x.Select(y =>
                    {
                        var e = new StringElement(y.Name);
                        e.Clicked.InvokeCommand(y.GoToCommand);
                        return e;
                    }));
                    Root.Reset(section);
                });
		
            OnActivation(d => {
                d(ViewModel.WhenAnyValue(x => x.SelectedFilter).Subscribe(x => viewSegment.SelectedSegment = x));
                d(viewSegment.GetChangedObservable().Subscribe(x => ViewModel.SelectedFilter = x));
            });
		}
	}
}

