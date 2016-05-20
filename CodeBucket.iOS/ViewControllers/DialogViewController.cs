//
// DialogViewController.cs: drives MonoTouch.Dialog
//
// Author:
//   Miguel de Icaza
//
// Code to support pull-to-refresh based on Martin Bowling's TweetTableView
// which is based in turn in EGOTableViewPullRefresh code which was created
// by Devin Doty and is Copyrighted 2009 enormego and released under the
// MIT X11 license
//
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using Foundation;
using CodeBucket.DialogElements;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers
{
    /// <summary>
    ///   The DialogViewController is the main entry point to use MonoTouch.Dialog,
    ///   it provides a simplified API to the UITableViewController.
    /// </summary>
    public class DialogViewController : TableViewController
    {
        private readonly ISubject<Unit> _endSubject = new Subject<Unit>();
        private readonly Lazy<RootElement> _rootElement;

        /// <summary>
        /// The root element displayed by the DialogViewController, the value can be changed during runtime to update the contents.
        /// </summary>
        public RootElement Root {
            get 
            {
                return _rootElement.Value;
            }
        } 

        public bool EnableSearch { get; set; }

        public string SearchPlaceholder { get; set; }

        public IObservable<Unit> EndOfList
        {
            get { return _endSubject.AsObservable(); }
        }

        public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate (fromInterfaceOrientation);
            ReloadData ();
        }

        Section [] originalSections;
        Element [][] originalElements;

        private void CreateOriginals(RootElement root)
        {
            originalSections = root.Sections.ToArray ();
            originalElements = new Element [originalSections.Length][];
            for (int i = 0; i < originalSections.Length; i++)
                originalElements [i] = originalSections [i].Elements.ToArray ();
        }

        ///// <summary>
        ///// Allows the caller to programatically stop searching.
        ///// </summary>
        //public virtual void FinishSearch ()
        //{
        //    if (originalSections == null)
        //        return;

        //    _searchBar.Text = "";

        //    Root.Reset(originalSections);
        //    originalSections = null;
        //    originalElements = null;
        //    _searchBar.ResignFirstResponder ();
        //    ReloadData ();
        //}

        public void PerformFilter (string text)
        {
            if (originalSections == null)
                return;

            var newSections = new List<Section> ();

            for (int sidx = 0; sidx < originalSections.Length; sidx++){
                Section newSection = null;
                var section = originalSections [sidx];
                Element [] elements = originalElements [sidx];

                for (int eidx = 0; eidx < elements.Length; eidx++){
                    if (elements [eidx].Matches (text)){
                        if (newSection == null){
                            newSection = new Section (section.Header, section.Footer){
                                FooterView = section.FooterView,
                                HeaderView = section.HeaderView
                            };
                            newSections.Add (newSection);
                        }
                        newSection.Add (elements [eidx]);
                    }
                }
            }

            Root.Reset(newSections);
            ReloadData ();
        }

        protected virtual void DidScroll(CGPoint p)
        {
        }

        public class Source : UITableViewSource {
            private readonly WeakReference<DialogViewController> _container;
            private readonly WeakReference<RootElement> _root;

            public RootElement Root
            {
                get { return _root.Get(); }
            }

            public DialogViewController Container
            {
                get { return _container.Get(); }
            }

            public Source (DialogViewController container)
            {
                _container = new WeakReference<DialogViewController>(container);
                _root = new WeakReference<RootElement>(container.Root);
            }

            public override nint RowsInSection (UITableView tableview, nint section)
            {
                var s = Root?[(int)section];
                var count = s?.Elements.Count;
                return count ?? 0;
            }

            public override nint NumberOfSections (UITableView tableView)
            {
                return Root?.Count ?? 0;
            }

            public override string TitleForHeader (UITableView tableView, nint section)
            {
                return Root?[(int)section]?.Header;
            }

            public override string TitleForFooter (UITableView tableView, nint section)
            {
                return Root?[(int)section]?.Footer;
            }

            public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
            {
                var section = Root?[indexPath.Section];
                var element = section?[indexPath.Row];
                return element?.GetCell (tableView);
            }

            public override void RowDeselected (UITableView tableView, NSIndexPath indexPath)
            {
                _container.Get()?.Deselected (indexPath);
            }

            public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
            {
                _container.Get()?.Selected (indexPath);
            }            

            public override UIView GetViewForHeader (UITableView tableView, nint sectionIdx)
            {
                var section = Root?[(int)sectionIdx];
                return section?.HeaderView;
            }

            public override nfloat GetHeightForHeader (UITableView tableView, nint sectionIdx)
            {
                var section = Root?[(int)sectionIdx];
                return section?.HeaderView?.Frame.Height ?? -1;
            }

            public override UIView GetViewForFooter (UITableView tableView, nint sectionIdx)
            {
                var section = Root?[(int)sectionIdx];
                return section?.FooterView;
            }

            public override nfloat GetHeightForFooter (UITableView tableView, nint sectionIdx)
            {
                var section = Root?[(int)sectionIdx];
                return section?.FooterView?.Frame.Height ?? -1;
            }

            public override void Scrolled (UIScrollView scrollView)
            {
                _container.Get()?.DidScroll(Root?.TableView?.ContentOffset ?? CGPoint.Empty);
            }

            public override void WillDisplay(UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
            {
                var s = tableView.NumberOfSections() - 1;
                var r = tableView.NumberOfRowsInSection(s) - 1;
                if (indexPath.Section == s && indexPath.Row == r)
                    _container.Get()?._endSubject.OnNext(Unit.Default);
            }

            public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
            {
                var section = Root?[indexPath.Section];
                var element = section?[indexPath.Row];
                var sizable = element as IElementSizing;
                return sizable?.GetHeight(tableView, indexPath) ?? tableView.RowHeight;
            }
        }

        public virtual void Deselected (NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];

            element.Deselected (TableView, indexPath);
        }

        public virtual void Selected (NSIndexPath indexPath)
        {
            var section = Root[indexPath.Section];
            var element = section[indexPath.Row];

            element.Selected (TableView, indexPath);
        }

        public virtual Source CreateSizingSource()
        {
            return new Source (this);
        }


        public override void LoadView ()
        {
            base.LoadView();
            TableView.Source = CreateSizingSource();
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            TableView.ReloadData ();
        }

        public void ReloadData ()
        {
            TableView.ReloadData();
        }

        public DialogViewController (UITableViewStyle style) 
            : base (style)
        {
            _rootElement = new Lazy<RootElement>(() => new RootElement(TableView));

            EdgesForExtendedLayout = UIRectEdge.None;
            SearchPlaceholder = "Search";
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "" };
        }
    }
}