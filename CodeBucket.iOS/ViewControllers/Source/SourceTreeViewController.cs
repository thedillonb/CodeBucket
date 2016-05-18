using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.DialogElements;
using System.Reactive.Linq;
using System.Linq;
using UIKit;
using CodeBucket.Views;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.Source
{
    public class SourceTreeViewController : BaseViewController<SourceTreeViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var tableView = new EnhancedTableView(UITableViewStyle.Plain);

            this.AddTableView(tableView);
            var root = new RootElement(tableView);
            tableView.Source = new DialogTableViewSource(root);

            ViewModel
                .Items
                .ChangedObservable()
                .Select(x => x.Select(CreateElement))
                .Subscribe(x => root.Reset(new Section { x }));
        }

        private static AtlassianIcon GetIcon(SourceTreeItemViewModel.SourceTreeItemType type)
        {
            switch (type)
            {
                case SourceTreeItemViewModel.SourceTreeItemType.Directory:
                    return AtlassianIcon.Devtoolsfolderclosed;
                case SourceTreeItemViewModel.SourceTreeItemType.File:
                    return AtlassianIcon.Devtoolsfile;
                default:
                    return AtlassianIcon.Devtoolsfilebinary;
            }
        }

        private static Element CreateElement(SourceTreeItemViewModel x)
        {
            var e = new StringElement(x.Name, GetIcon(x.Type).ToImage());
            e.Clicked.Select(_ => x).BindCommand(x.GoToCommand);
            return e;
        }
    }
}

