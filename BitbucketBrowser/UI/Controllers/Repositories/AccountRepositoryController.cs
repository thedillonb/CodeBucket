using System;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using System.Linq;
using BitbucketSharp.Models;
using System.Collections.Generic;

namespace BitbucketBrowser.UI.Controllers.Repositories
{
    public class AccountRepositoryController : RepositoryController
    {
        private UISegmentedControl _segment = new UISegmentedControl(new [] { "Owned", "Following" });
        
        public AccountRepositoryController(string username)
            : base(username)
        {
        }
        
        protected override void OnRefresh ()
        {
            if (Root != null)
                InvokeOnMainThread(delegate { Root.Clear(); });
            
            if (Model.Count == 0)
                return;
            
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            
            var sec = new Section();
            Model.ForEach(x => {
                RepositoryElement sse = new RepositoryElement(x) { ShowOwner = selected != 0 };
                sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
                sec.Add(sse);
            });
            
            //Sort them by name
            sec.Elements = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name).ToList();

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }
        
        protected override List<RepositoryDetailedModel> OnUpdate (bool forced)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            
            if (selected == 0)
                return Application.Client.Users[Username].GetInfo(forced).Repositories;
            else if (selected == 1)
                return Application.Client.Account.GetRepositories(forced);
            else
                return new List<RepositoryDetailedModel>();
        }
        
        
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            
            _segment.ControlStyle = UISegmentedControlStyle.Bar;
            _segment.SelectedSegment = 0;
            
            //Fucking bug in the divider
            BeginInvokeOnMainThread(delegate {
                _segment.SelectedSegment = 1;
                _segment.SelectedSegment = 0;
                _segment.ValueChanged += (sender, e) => ChangeSegment();
            });
            
            
            //NavigationItem.TitleView = _segment;
            
            Title = "Owned";
            
            //The bottom bar
            var btn = new UIBarButtonItem(_segment);
            btn.Width = View.Frame.Width - 10f;
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), btn, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }
        
        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(isSearching, true);
            base.ViewWillAppear(animated);
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, true);
        }
        
        private void ChangeSegment()
        {
            Root.Clear(); 
            TableView.TableFooterView.Hidden = true;
            Model = null;
            Refresh();
            
            if (_segment.SelectedSegment == 0)
                Title = "Owned";
            else if (_segment.SelectedSegment == 1)
                Title = "Following";
            else if (_segment.SelectedSegment == 2)
                Title = "Viewed";
        }
        
        protected override void SearchStart()
        {
            NavigationController.SetToolbarHidden(true, false);
        }
        
        protected override void SearchEnd()
        {
            NavigationController.SetToolbarHidden(false, false);
        }
    }

}

