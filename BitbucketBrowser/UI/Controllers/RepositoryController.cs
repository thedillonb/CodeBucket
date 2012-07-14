using System.Threading;
using BitbucketSharp;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace BitbucketBrowser.UI
{
    public class AccountRepositoryController : UIViewController
    {
        private UISegmentedControl _segment = new UISegmentedControl(new [] { "Following", "Mine" });
        private RepositoryController _owned;
        private MyRepo _followed;

        public string Username { get; private set; }

        public AccountRepositoryController(string username)
        {
            Username = username;
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            _owned = new RepositoryController(Username, false) { Nav = this.NavigationController };
            _followed = new MyRepo(Username) { Nav = this.NavigationController };

            View.AddSubview(_owned.View);
            View.AddSubview(_followed.View);

            _segment.ControlStyle = UISegmentedControlStyle.Bar;
            _segment.SelectedSegment = 0;
            _segment.ValueChanged += (sender, e) => ChangeSegment();
            NavigationItem.TitleView = _segment;

            var cur = _segment.SelectedSegment;
            _owned.View.Hidden = cur == 0;
            _followed.View.Hidden = !_owned.View.Hidden;

            WatermarkView.AssureWatermark(this);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (_segment.SelectedSegment == 0)
            {
                if (!_followed.Loaded)
                    _followed.Refresh();
            }
            else if (_segment.SelectedSegment == 1)
            {
                if (!_owned.Loaded)
                    _owned.Refresh();
            }
        }

        private void ChangeSegment()
        {
            var cur = _segment.SelectedSegment;
            _owned.View.Hidden = cur == 0;
            _followed.View.Hidden = !_owned.View.Hidden;
            if (_segment.SelectedSegment == 0)
            {
                if (!_followed.Loaded)
                    _followed.Refresh();
            }
            else if (_segment.SelectedSegment == 1)
            {
                if (!_owned.Loaded)
                    _owned.Refresh();
            }
        }

        private class MyRepo : RepositoryController
        {
            public MyRepo(string username)
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