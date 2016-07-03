using System;
using ReactiveUI;

namespace CodeBucket.TableViewCells
{
    public abstract class BaseTableViewCell<TViewModel> : ReactiveTableViewCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected BaseTableViewCell()
        {
        }

        protected BaseTableViewCell(UIKit.UITableViewCellStyle style, string reuseIdentifier) 
            : base(style, reuseIdentifier)
        {
        }

        protected BaseTableViewCell(IntPtr handle)
            : base(handle)
        {
        }

        private TViewModel _viewModel;
        public TViewModel ViewModel
        {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }
    }
}

