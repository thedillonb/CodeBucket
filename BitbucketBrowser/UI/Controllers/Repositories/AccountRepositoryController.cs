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
        private static string SavedSelection = "REPO_SELECTION";
        private UISegmentedControl _segment = new UISegmentedControl(new [] { "Owned", "Following" });
        private UIBarButtonItem _segmentBarButton;
        
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
        
        protected override List<RepositoryDetailedModel> OnUpdate(bool forced)
        {
            var selected = 0;
            InvokeOnMainThread(() => { selected = _segment.SelectedSegment; });
            
            if (selected == 0)
            {
                var userInfo = Application.Client.Users[Username].GetInfo(forced);
                Application.Account.AvatarUrl = userInfo.User.Avatar;
                BeginInvokeOnMainThread(() => Application.Account.Update());
                return userInfo.Repositories;
            }
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
            _segment.AutosizesSubviews = true;
            _segment.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            //Fucking bug in the divider
            BeginInvokeOnMainThread(delegate {
                _segment.SelectedSegment = 1;
                _segment.SelectedSegment = 0;
                _segment.SelectedSegment = MonoTouch.Utilities.Defaults.IntForKey(SavedSelection);
                SetTitle();
                _segment.ValueChanged += (sender, e) => ChangeSegment();
            });
            
            Title = "Owned";
            
            //The bottom bar
            _segmentBarButton = new UIBarButtonItem(_segment);
            ToolbarItems = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), _segmentBarButton, new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };
        }

        public override void ViewDidLayoutSubviews()
        {
            //Resize and refresh the toolbar items by assigning it to itself!
            _segmentBarButton.Width = View.Bounds.Width - 16f;
            ToolbarItems = ToolbarItems;
            base.ViewDidLayoutSubviews();
        }
        
        public override void ViewWillAppear(bool animated)
        {
            NavigationController.SetToolbarHidden(IsSearching, animated);
            base.ViewWillAppear(animated);
        }
        
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, animated);
        }
        
        private void ChangeSegment()
        {
            Root.Clear(); 
            TableView.TableFooterView.Hidden = true;
            Model = null;
            Refresh();
            
            SetTitle();

            //Remember the default value!
            MonoTouch.Utilities.Defaults.SetInt(_segment.SelectedSegment, SavedSelection);
            MonoTouch.Utilities.Defaults.Synchronize();
        }

        private void SetTitle()
        {
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
            base.SearchStart();
        }
        
        protected override void SearchEnd()
        {
            NavigationController.SetToolbarHidden(false, false);
            base.SearchEnd();
        }
    }

}

