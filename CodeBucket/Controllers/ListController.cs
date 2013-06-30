using System;
using CodeBucket.Elements;
using MonoTouch.Dialog;
using MonoTouch;
using System.Collections.Generic;
using MonoTouch.UIKit;

namespace CodeBucket.Controllers
{
    public abstract class ListController<T> : Controller<List<T>>
    {
        protected int _nextPage = 1;
        private LoadMoreElement _loadMore;
        protected UISegmentedControl _segment;

        protected string[] MultipleSelections { get; set; }
        protected string MultipleSelectionsKey { get; set; }

        public bool UnevenRows { get; set; }
        public bool AppendStrategy { get; set; }

        protected ListController(bool push = false, bool refresh = true)
            : base(push, refresh)
        {
            Style = UITableViewStyle.Plain;
            EnableSearch = true;
            AutoHideSearch = true;
            AppendStrategy = true;
        }

        protected override List<T> OnUpdate(bool forced)
        {
            _nextPage = 1;
            return GetData(forced, _nextPage, out _nextPage);
        }

        protected override void OnRefresh()
        {
            var sec = new Section();
            if (Model.Count == 0)
                sec.Add(new NoItemsElement());
            else
                Model.ForEach(x => sec.Add(CreateElement(x)));

            Section loadSec = null;
            if (_nextPage >= 0)
            {
                _loadMore = new PaginateElement("Load More", "Loading...", e => this.DoWorkNoHud(LoadWork, LoadException, LoadFinished));
                loadSec = new Section() { _loadMore };
            }

            InvokeOnMainThread(delegate {
                var r = new RootElement(Title) { UnevenRows = UnevenRows };
                r.Add(sec);
                if (loadSec != null)
                    r.Add(loadSec);
                Root = r;
            });
        }

        protected abstract List<T> GetData(bool force, int currentPage, out int nextPage);

        protected abstract Element CreateElement(T obj);

        private void LoadWork()
        {
            var data = GetData(true, _nextPage, out _nextPage);
            if (Model == null)
                Model = data;
            else
                Model.AddRange(data);

            if (AppendStrategy)
            {
                var sec = new Section();
                data.ForEach(x => sec.Add(CreateElement(x)));
                InvokeOnMainThread(() => {
                    Root.Insert(Root.Count - 1, sec);
                });
            } 
            else
            {
                InvokeOnMainThread(() => {
                    var c = TableView.ContentOffset;
                    OnRefresh();
                    TableView.ContentOffset = c;
                });
            }
        }

        private void LoadException(Exception ex)
        {
            Utilities.ShowAlert("Failure to load!", "Unable to load additional enries because the following error: " + ex.Message);
        }

        private void LoadFinished()
        {
            if (_loadMore != null)
            {
                _loadMore.Animating = false;
                if (_nextPage < 0)
                {
                    Root.Remove(_loadMore.Parent as Section);
                    _loadMore.Dispose();
                    _loadMore = null;
                }
            }
        }

        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (MultipleSelections != null)
            {
                _segment = new UISegmentedControl(MultipleSelections);
                _segment.ControlStyle = UISegmentedControlStyle.Bar;
                _segment.SelectedSegment = 0;
                _segment.AutosizesSubviews = true;
                _segment.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

                //Fucking bug in the divider
                BeginInvokeOnMainThread(delegate {
                    _segment.SelectedSegment = 1;
                    _segment.SelectedSegment = 0;
                    _segment.SelectedSegment = MonoTouch.Utilities.Defaults.IntForKey(MultipleSelectionsKey);
                    Title = GetTitle(_segment.SelectedSegment);
                    _segment.ValueChanged += (sender, e) => ChangeSegment();
                });
            
                Title = GetTitle(0);
            
                //The bottom bar
                ToolbarItems = new []
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                new UIBarButtonItem(_segment),
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };
            }
        }
        
        private void ChangeSegment()
        {
            Root.Clear(); 
            TableView.TableFooterView.Hidden = true;
            Model = null;
            Refresh();
            
            Title = GetTitle(_segment.SelectedSegment);
            
            //Remember the default value!
            MonoTouch.Utilities.Defaults.SetInt(_segment.SelectedSegment, MultipleSelectionsKey);
            MonoTouch.Utilities.Defaults.Synchronize();
        }
        
        protected virtual string GetTitle(int selection)
        {
            if (selection >= MultipleSelections.Length || selection < 0)
                return Title;
            return MultipleSelections[selection];
        }
        
        protected override void SearchStart()
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, false);
            base.SearchStart();
        }
        
        protected override void SearchEnd()
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(false, false);
            base.SearchEnd();
        }
        
        public override void ViewDidLayoutSubviews()
        {
            //Resize and refresh the toolbar items by assigning it to itself!
            if (ToolbarItems != null)
            {
                ToolbarItems [1].Width = View.Bounds.Width - 16f;
                ToolbarItems = ToolbarItems;
            }
            base.ViewDidLayoutSubviews();
        }
        
        public override void ViewWillAppear(bool animated)
        {
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(IsSearching, animated);
            base.ViewWillAppear(animated);
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (ToolbarItems != null)
                NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

