using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CodeBucket.DialogElements;
using Foundation;
using UIKit;

namespace CodeBucket.TableViewSources
{
    public class DialogElementTableViewSource : UITableViewSource
    {
        private readonly ISubject<Unit> _endSubject = new Subject<Unit>();
        private readonly RootElement _root;

        public RootElement Root
        {
            get { return _root.Get(); }
        }

        public IObservable<Unit> EndOfList
        {
            get { return _endSubject.AsObservable(); }
        }

        public DialogElementTableViewSource(UITableView tableView)
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
            var cell = element?.GetCell(tableView);
            if (cell != null && cell.Hidden != element.Hidden)
                cell.Hidden = element.Hidden;
            return cell;
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

        public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
        {
            var s = tableView.NumberOfSections() - 1;
            var r = tableView.NumberOfRowsInSection(s) - 1;
            if (indexPath.Section == s && indexPath.Row == r)
                _endSubject.OnNext(Unit.Default);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var section = Root?[indexPath.Section];
            var element = section?[indexPath.Row];

            if (element?.Hidden ?? false)
            {
                return 0;
            }

            var sizable = element as IElementSizing;
            return sizable?.GetHeight(tableView, indexPath) ?? tableView.RowHeight;
        }
    }
}

