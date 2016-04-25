using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels
{
    public class CollectionViewModel<TItem> : ReactiveObject
    {
        private readonly ReactiveList<TItem> _source;
        private Func<IEnumerable<TItem>, IEnumerable<IGrouping<string, TItem>>> _groupingFunction;
        private Func<IEnumerable<TItem>, IEnumerable<TItem>> _sortingFunction;
        private Func<IEnumerable<TItem>, IEnumerable<TItem>> _filteringFunction;
        private Func<Task> _moreItems;

        public ReactiveList<TItem> Items
        {
            get { return _source; }
        }

        public Func<Task> MoreItems
        {
            get { return _moreItems; }
            set { this.RaiseAndSetIfChanged(ref _moreItems, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> SortingFunction
        {
            get { return _sortingFunction; }
            set { this.RaiseAndSetIfChanged(ref _sortingFunction, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<TItem>> FilteringFunction
        {
            get { return _filteringFunction; }
            set { this.RaiseAndSetIfChanged(ref _filteringFunction, value); }
        }

        public Func<IEnumerable<TItem>, IEnumerable<IGrouping<string, TItem>>> GroupingFunction
        {
            get { return _groupingFunction; }
            set { this.RaiseAndSetIfChanged(ref _groupingFunction, value); }
        }

        public CollectionViewModel()
        {
            _source = new ReactiveList<TItem>();
        }

        public CollectionViewModel(IEnumerable<TItem> source)
        {
            _source = new ReactiveList<TItem>(source);
        }
    }
}

