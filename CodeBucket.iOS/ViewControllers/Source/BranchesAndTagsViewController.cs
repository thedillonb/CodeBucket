using CodeBucket.Core.ViewModels.Source;
using UIKit;
using System;
using CodeBucket.Views;
using ReactiveUI;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesAndTagsViewController : BaseTableViewController<BranchesAndTagsViewModel>
	{
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var viewSegment = new UISegmentedControl(new object[] {"Branches", "Tags"});
            NavigationItem.TitleView = viewSegment;

            TableView.Source = new ReferenceTableViewSource(TableView, ViewModel.Items);
            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Filecode.ToEmptyListImage(), "There are no items."));

            OnActivation(d => 
            {
                this.WhenAnyValue(x => x.ViewModel.SelectedFilter)
                    .Subscribe(x => viewSegment.SelectedSegment = x)
                    .AddTo(d);
                
                viewSegment.GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = x)
                    .AddTo(d);
            });
		}
	}
}

