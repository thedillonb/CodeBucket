using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Views.Filters;
using UIKit;
using System;
using Foundation;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using MvvmCross.iOS.Views;

namespace CodeBucket.Views.Issues
{
    public class IssuesFiltersView : ViewModelCollectionDrivenDialogViewController, IMvxModalIosView
    {
        public new IssuesFiltersViewModel ViewModel
        {
            get { return (IssuesFiltersViewModel)base.ViewModel; }
            set { base.ViewModel = value; }
        }

        public IssuesFiltersView()
        {
            EnableSearch = false;
            Title = "Filters";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationItem.RightBarButtonItem = new UIKit.UIBarButtonItem(UIKit.UIBarButtonSystemItem.Add, (s, e) => CreateNewFilter());
//            BindCollection(ViewModel.Filters, x => {
//                var element = new FilterElement(x, () => EditFilter(x));
//                element.Clicked.Select(_ => x).BindCommand(ViewModel.SelectFilterCommand);
//                return element;
//            }, true);
        }

        private void CreateNewFilter()
        {
            var ctrl = new IssuesFilterViewController(new CodeBucket.Core.Filters.IssuesFilterModel());
            ctrl.CreatedFilterModel = ViewModel.NewFilterCommand.Execute;
            NavigationController.PushViewController(ctrl, true);
        }

        private void EditFilter(IssuesFiltersViewModel.FilterModel filterModel)
        {
            var ctrl = new IssuesFilterViewController(filterModel.IssueModel);
            ctrl.CreatedFilterModel = x =>
            {
                filterModel.IssueModel = x;
                ViewModel.EditFilterCommand.Execute(filterModel);
            };
            NavigationController.PushViewController(ctrl, true);
        }

        private class FilterElement : StringElement
        {
            public IssuesFiltersViewModel.FilterModel FilterModel { get; private set; }
            public FilterElement(IssuesFiltersViewModel.FilterModel filterModel)
                : base(filterModel.IssueModel.FilterName)
            {
                Accessory = UITableViewCellAccessory.DetailButton;
//                AccessoryTapped += () => accessory();
                FilterModel = filterModel;
            }
        }

        public override DialogViewController.Source CreateSizingSource()
        {
            return new EditSource(this);
        }

        private class EditSource : Source
        {
            private readonly IssuesFiltersView _parent;
            public EditSource(IssuesFiltersView dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
            {
                return _parent.ViewModel.Filters.Items.Count > 0;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
            {
                switch (editingStyle)
                {
                    case UITableViewCellEditingStyle.Delete:
                        var section = _parent.Root[indexPath.Section];
                        var element = section[indexPath.Row];
                        section.Remove(element);
                        _parent.ViewModel.RemoveFilterCommand.Execute(((FilterElement)element).FilterModel);
                        break;
                }
            }
        }
    }
}

