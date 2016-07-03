using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.TableViewCells;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class TeamTableViewSource : BaseTableViewSource<TeamItemViewModel>
    {
        public TeamTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<TeamItemViewModel> collection)
            : base(tableView, collection, TeamTableViewCell.Key, 44)
        {
            tableView.RegisterClassForCellReuse(typeof(TeamTableViewCell), TeamTableViewCell.Key);
        }
    }
}

