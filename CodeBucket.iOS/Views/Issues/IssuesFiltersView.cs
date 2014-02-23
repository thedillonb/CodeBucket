using Cirrious.MvvmCross.Touch.Views;
using CodeBucket.Core.ViewModels.Issues;
using MonoTouch.Dialog;
using CodeFramework.ViewControllers;
using CodeBucket.iOS.Views.Filters;
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;

namespace CodeBucket.iOS.Views.Issues
{
    public class IssuesFiltersView : ViewModelCollectionDrivenDialogViewController, IMvxModalTouchView
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
            NavigationItem.RightBarButtonItem = new MonoTouch.UIKit.UIBarButtonItem(MonoTouch.UIKit.UIBarButtonSystemItem.Add, (s, e) => CreateNewFilter());
            BindCollection(ViewModel.Filters, x => new FilterElement(x, () => ViewModel.SelectFilterCommand.Execute(x), () => EditFilter(x)), true);
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

        private class FilterElement : StyledStringElement
        {
            public IssuesFiltersViewModel.FilterModel FilterModel { get; private set; }
            public FilterElement(IssuesFiltersViewModel.FilterModel filterModel, NSAction action, Action accessory)
                : base(filterModel.IssueModel.FilterName, action)
            {
                Accessory = UITableViewCellAccessory.DetailButton;
                AccessoryTapped += () => accessory();
                FilterModel = filterModel;
            }
        }

        public override DialogViewController.Source CreateSizingSource(bool unevenRows)
        {
            return new EditSource(this);
        }

        private class EditSource : SizingSource
        {
            private readonly IssuesFiltersView _parent;
            public EditSource(IssuesFiltersView dvc) 
                : base (dvc)
            {
                _parent = dvc;
            }

            public override bool CanEditRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return _parent.ViewModel.Filters.Items.Count > 0;
            }

            public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
            {
                return UITableViewCellEditingStyle.Delete;
            }

            public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
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

