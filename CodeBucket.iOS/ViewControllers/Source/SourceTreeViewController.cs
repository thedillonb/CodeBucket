using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewSources;
using CodeBucket.Views;
using UIKit;

namespace CodeBucket.ViewControllers.Source
{
    public class SourceTreeViewController : BaseTableViewController<SourceTreeViewModel, SourceTreeItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolsfile.ToEmptyListImage(), "There is no content."));
            TableView.Source = new SourceTreeTableViewSource(TableView, ViewModel.Items);
        }
    }
}

