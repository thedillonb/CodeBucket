using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeBucket.Core.ViewModels;
using CoreGraphics;
using ReactiveUI;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public abstract class BaseTableViewSource<TViewModel> : ReactiveTableViewSource<TViewModel>
    {
        private readonly Subject<Unit> _requestMoreSubject = new Subject<Unit>();
        private readonly Subject<CGPoint> _scrollSubject = new Subject<CGPoint>();

        public IObservable<CGPoint> DidScroll
        {
            get { return _scrollSubject.AsObservable(); }
        }

        public IObservable<Unit> RequestMore
        {
            get { return _requestMoreSubject; }
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _scrollSubject.OnNext(scrollView.ContentOffset);
        }

        ~BaseTableViewSource()
        {
            Console.WriteLine("Destorying " + GetType().Name);
        }

        protected BaseTableViewSource(UITableView tableView, nfloat height, nfloat? heightHint = null)
            : base(tableView)
        {
            tableView.RowHeight = height;
            tableView.EstimatedRowHeight = heightHint ?? tableView.EstimatedRowHeight;
        }

        protected BaseTableViewSource(UITableView tableView, IReactiveNotifyCollectionChanged<TViewModel> collection,
            Foundation.NSString cellKey, nfloat height, nfloat? heightHint = null, Action<UITableViewCell> initializeCellAction = null) 
            : base(tableView, collection, cellKey, (float)height, initializeCellAction)
        {
            tableView.RowHeight = height;
            tableView.EstimatedRowHeight = heightHint ?? tableView.EstimatedRowHeight;
        }

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, Foundation.NSIndexPath indexPath)
        {
            if (indexPath.Section == (NumberOfSections(tableView) - 1) &&
                indexPath.Row == (RowsInSection(tableView, indexPath.Section) - 1))
            {
                // We need to skip an event loop to stay out of trouble
                BeginInvokeOnMainThread(() => _requestMoreSubject.OnNext(Unit.Default));
            }
        }

        public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
        {
            var item = ItemAt(indexPath) as ICanGoToViewModel;
            item?.GoToCommand.ExecuteIfCan();
            base.RowSelected(tableView, indexPath);
            tableView.DeselectRow(indexPath, true);
        }

        protected override void Dispose(bool disposing)
        {
            _requestMoreSubject.Dispose();
            _scrollSubject.Dispose();
            base.Dispose(disposing);
        }
    }

    public interface IInformsEnd
    {
        IObservable<Unit> RequestMore { get; }
    }
}

