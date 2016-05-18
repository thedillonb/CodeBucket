using CodeBucket.Core.ViewModels.Teams;
using CodeBucket.DialogElements;
using System;
using UIKit;
using CodeBucket.Views;
using System.Linq;
using CodeBucket.TableViewSources;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Teams
{
    public class TeamsViewController : BaseViewController<TeamsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableView = new EnhancedTableView(UITableViewStyle.Plain)
            {
                EmptyView = new Lazy<UIView>(() =>
                    new EmptyListView(AtlassianIcon.Userstatus.ToEmptyListImage(), "There are no teams."))
            };

            this.AddTableView(tableView);
            var root = new RootElement(tableView);
            tableView.Source = new DialogTableViewSource(root);

            ViewModel.Items.ChangedObservable()
                 .Select(x => x.Select(CreateElement))
                 .Subscribe(x => root.Reset(new Section { x }));
        }

        private static Element CreateElement(TeamItemViewModel vm)
        {
            var e = new StringElement(vm.Name);
            e.BindClick(vm.GoToCommand);
            return e;
        }
    }
}