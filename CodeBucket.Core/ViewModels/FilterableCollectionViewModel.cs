using MvvmCross.Platform;
using CodeBucket.Core.Services;
using CodeBucket.Core.Filters;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public class FilterableCollectionViewModel<T, TF> : CollectionViewModel<T>, IFilterableViewModel<TF> where TF : FilterModel<TF>, new()
    {
        protected TF _filter;
        private readonly string _filterKey;

        public TF Filter
        {
            get { return _filter; }
            set { this.RaiseAndSetIfChanged(ref _filter, value); }

        }

        public FilterableCollectionViewModel(string filterKey)
        {
            _filterKey = filterKey;
            var accounts = Mvx.Resolve<IAccountsService>();
            _filter = accounts.ActiveAccount.Filters.GetFilter<TF>(_filterKey);
        }

        public void ApplyFilter(TF filter, bool saveAsDefault = false)
        {
            Filter = filter;
            if (saveAsDefault)
            {
                var accounts = Mvx.Resolve<IAccountsService>();
                accounts.ActiveAccount.Filters.AddFilter(_filterKey, filter);
            }
        }
    }
}

