using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewSources;
using CodeBucket.Views;
using UIKit;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesViewController : BaseTableViewController<BranchesViewModel, GitReferenceItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsbranch.ToEmptyListImage(), "There are no branches."));
            TableView.Source = new ReferenceTableViewSource(TableView, ViewModel.Items);
        }
    }
}

