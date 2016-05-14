using CodeBucket.Core.ViewModels.Groups;
using System;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Views;
using System.Linq;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Groups
{
    public class GroupsViewController : BaseViewController<GroupsViewModel>
	{
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableView = new EnhancedTableView(UITableViewStyle.Plain)
            {
                ViewModel = ViewModel,
                EmptyView = new Lazy<UIView>(() =>
                    new EmptyListView(AtlassianIcon.Group.ToEmptyListImage(), "There are no groups."))
            };

            this.AddTableView(tableView);
            var root = new RootElement(tableView);
            tableView.Source = new DialogElementTableViewSource(root);

            ViewModel
                .Items
                .ChangedObservable()
                .Subscribe(groups =>
                {
                    var elements = groups.Select(x =>
                    {
                        var e = new StringElement(x.Name);
                        e.BindClick(x.GoToCommand);
                        return e;
                    });

                    root.Reset(new Section { elements });
                });
        }
	}
}

