using System.Threading;
using BitbucketSharp;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace BitbucketBrowser.UI
{
    public class AccountRepositoryController : UIViewController
    {
        private UISegmentedControl _segment = new UISegmentedControl(new [] { "Owned", "Following", "Viewed" });
        private RepositoryController _owned;
        private FollowingRepositoryController _followed;

        public string Username { get; private set; }

        public AccountRepositoryController(string username)
        {
            Username = username;
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _owned = new RepositoryController(Username, false) { Nav = this.NavigationController, Title = "Owned Repos" };
            _owned.View.Frame = new RectangleF(_owned.View.Frame.X, _owned.View.Frame.Y, _owned.View.Frame.Width, _owned.View.Frame.Height - 44f);

            _followed = new FollowingRepositoryController(Username) { Nav = this.NavigationController, Title = "Following Repos" };
            _followed.View.Frame = new RectangleF(_followed.View.Frame.X, _followed.View.Frame.Y, _followed.View.Frame.Width, _followed.View.Frame.Height - 44f);

            View.AddSubview(_owned.View);
            View.AddSubview(_followed.View);

            _segment.ControlStyle = UISegmentedControlStyle.Bar;
            _segment.SelectedSegment = 0;
            _segment.ValueChanged += (sender, e) => ChangeSegment();
            //NavigationItem.TitleView = _segment;

            _owned.View.Hidden = false;
            _followed.View.Hidden = !_owned.View.Hidden;

            WatermarkView.AssureWatermark(this);

            var btn = new UIBarButtonItem(_segment);
            btn.Width = View.Frame.Width - 10f;

            var changeBar = new UIToolbar(new RectangleF(0, View.Frame.Height - 44f * 2, View.Frame.Width, 44f));
            changeBar.SetBackgroundImage(Images.Bottombar, UIToolbarPosition.Any, UIBarMetrics.Default);
            changeBar.Items = new [] { new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace), btn , new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) };


            View.AddSubview(changeBar);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_segment.SelectedSegment == 1)
            {
                if (!_followed.Loaded)
                    _followed.Refresh();
                Title = _followed.Title;
            }
            else if (_segment.SelectedSegment == 0)
            {
                if (!_owned.Loaded)
                    _owned.Refresh();
                Title = _owned.Title;
            }
        }

        private void ChangeSegment()
        {
            var cur = _segment.SelectedSegment;
            _owned.View.Hidden = cur == 1;
            _followed.View.Hidden = !_owned.View.Hidden;
            if (_segment.SelectedSegment == 1)
            {
                if (!_followed.Loaded)
                    _followed.Refresh();
                Title = _followed.Title;
            }
            else if (_segment.SelectedSegment == 0)
            {
                if (!_owned.Loaded)
                    _owned.Refresh();
                Title = _owned.Title;
            }
        }

        private class FollowingRepositoryController : RepositoryController
        {
            public FollowingRepositoryController(string username)
                : base(username, false)
            {
            }

            protected override List<RepositoryDetailedModel> OnUpdate()
            {
                return Application.Client.Account.GetRepositories();
            }
        }

    }

    public class RepositoryController : Controller<List<RepositoryDetailedModel>>
    {
        public string Username { get; private set; }

        public UINavigationController Nav { get; set; }

        public RepositoryController(string username, bool push = true) 
            : base(push, true)
        {
            Title = "Repositories";
            Style = UITableViewStyle.Plain;
            Username = username;
            AutoHideSearch = true;
            EnableSearch = true;
        } 


        protected override void OnRefresh()
        {
            if (Model.Count == 0)
                return;

            var sec = new Section();
            Model.ForEach(x => {
                RepositoryElement sse = new RepositoryElement(x);
                sse.Tapped += () => Nav.PushViewController(new RepositoryInfoController(x), true);
                sec.Add(sse);
            });

            //Sort them by name
            sec.Elements = sec.Elements.OrderBy(x => ((RepositoryElement)x).Model.Name).ToList();

            InvokeOnMainThread(delegate {
                Root = new RootElement(Title) { sec };
            });
        }

        protected override List<RepositoryDetailedModel> OnUpdate()
        {
            return Application.Client.Users[Username].GetInfo().Repositories;
        }
    }
}