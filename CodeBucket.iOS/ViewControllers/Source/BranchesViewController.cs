using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesViewController : BaseTableViewController<BranchesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.Source = new ReferenceTableViewSource(TableView, ViewModel.Branches);
        }
    }
}

