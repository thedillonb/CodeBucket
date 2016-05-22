using System;
using UIKit;
using System.Collections.Generic;
using Foundation;
using CodeBucket.DialogElements;
using System.Linq;
using System.Reactive.Linq;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers
{
    public class DialogViewController : TableViewController
    {
        private readonly Lazy<RootElement> _rootElement;

        public RootElement Root => _rootElement.Value;

        public bool EnableSearch { get; set; }

        public string SearchPlaceholder { get; set; }

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

        public virtual UITableViewSource CreateSizingSource()
        {
            return new DialogTableViewSource(Root);
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