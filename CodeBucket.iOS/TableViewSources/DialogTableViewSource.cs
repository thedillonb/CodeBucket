using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeBucket.DialogElements;
using CoreGraphics;
using Foundation;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class DialogTableViewSource : UITableViewSource
    {
        private readonly WeakReference<RootElement> _root;
        private readonly ISubject<CGPoint> _scrolledObservable = new Subject<CGPoint>();

        public RootElement Root
        {
            get { return _root.Get(); }
        }

        public IObservable<CGPoint> DidScrolled => _scrolledObservable.AsObservable();

        public DialogTableViewSource(RootElement rootElement)
        {
            _root = new WeakReference<RootElement>(rootElement);
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            var s = Root?[(int)section];
            var count = s?.Elements.Count;
            return count ?? 0;
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            return Root?.Count ?? 0;
        }

        public override string TitleForHeader(UITableView tableView, nint section)
        {
            return Root?[(int)section]?.Header;
        }

        public override string TitleForFooter(UITableView tableView, nint section)
        {
            return Root?[(int)section]?.Footer;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root?[indexPath.Section];
            var element = section?[indexPath.Row];
            return element?.GetCell(tableView);
        }

        public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root?.Sections.ElementAtOrDefault(indexPath.Section);
            var element = section?.Elements.ElementAtOrDefault((int)indexPath.Item);
            element?.Deselected(tableView, indexPath);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root?.Sections.ElementAtOrDefault(indexPath.Section);
            var element = section?.Elements.ElementAtOrDefault((int)indexPath.Item);
            element?.Selected(tableView, indexPath);
        }

        public override UIView GetViewForHeader(UITableView tableView, nint sectionIdx)
        {
            var section = Root?[(int)sectionIdx];
            return section?.HeaderView;
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint sectionIdx)
        {
            var section = Root?[(int)sectionIdx];
            return section?.HeaderView?.Frame.Height ?? -1;
        }

        public override UIView GetViewForFooter(UITableView tableView, nint sectionIdx)
        {
            var section = Root?[(int)sectionIdx];
            return section?.FooterView;
        }

        public override nfloat GetHeightForFooter(UITableView tableView, nint sectionIdx)
        {
            var section = Root?[(int)sectionIdx];
            return section?.FooterView?.Frame.Height ?? -1;
        }

        public override void Scrolled(UIScrollView scrollView)
        {
            _scrolledObservable.OnNext(Root?.TableView?.ContentOffset ?? CGPoint.Empty);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root?[indexPath.Section];
            var element = section?[indexPath.Row];
            var sizable = element as IElementSizing;
            return sizable?.GetHeight(tableView, indexPath) ?? tableView.RowHeight;
        }
    }
}

