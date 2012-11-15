using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeFramework.UI.Controllers;
using CodeFramework.UI.Elements;
using System;

namespace BitbucketBrowser.UI.Controllers.Repositories
{
    public class RepositoryController : Controller<List<RepositoryDetailedModel>>
    {
        FilterModel _filterModel = Application.Account.RepoFilterObject;
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public RepositoryController(string username, bool push = true)
            : this(username, push, true)
        {
        }

        public RepositoryController(string username, bool push = true, bool refresh = true)
            : base(push, refresh)
        {
            Title = "Repositories";
            Style = UITableViewStyle.Plain;
            Username = username;
            AutoHideSearch = true;
            EnableSearch = true;
            ShowOwner = true;
            EnableFilter = true;
            SearchPlaceholder = "Search Repositories";
        }

        private RepositoryElement CreateElement(RepositoryDetailedModel x)
        {
            var sse = new RepositoryElement(x) { ShowOwner = ShowOwner };
            sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(x), true);
            return sse;
        }

        protected override void OnRefresh()
        {
            if (Model == null)
                return;

            var order = (FilterModel.Order)_filterModel.OrderBy;
            IEnumerable<RepositoryDetailedModel> results;
            if (order == FilterModel.Order.Forks)
                results = Model.OrderBy(x => x.ForkCount);
            else if (order == FilterModel.Order.LastUpdated)
                results = Model.OrderBy(x => x.UtcLastUpdated);
            else if (order == FilterModel.Order.CreatedOn)
                results = Model.OrderBy(x => x.UtcCreatedOn);
            else if (order == FilterModel.Order.Followers)
                results = Model.OrderBy(x => x.FollowersCount);
            else if (order == FilterModel.Order.Owner)
                results = Model.OrderBy(x => x.Owner);
            else
                results = Model.OrderBy(x => x.Name);

            if (!_filterModel.Ascending)
                results = results.Reverse();

            var section = new Section();
            foreach (var x in results)
                section.Add(CreateElement(x));


            if (section.Count == 0)
                section.Add(new NoItemsElement("No Repositories"));

            InvokeOnMainThread(() => { Root = new RootElement(Title) { section }; });
        }

        protected override List<RepositoryDetailedModel> OnUpdate(bool forced)
        {
            return Application.Client.Users[Username].GetInfo(forced).Repositories;
        }

        private void ApplyFilter()
        {
            OnRefresh();
        }

        protected override FilterController CreateFilterController()
        {
            return new Filter(this);
        }

        public class FilterModel
        {
            public int OrderBy { get; set; }
            public bool Ascending { get; set; }
            
            public FilterModel()
            {
                OrderBy = (int)Order.Name;
                Ascending = true;
            }
            
            public enum Order : int
            { 
                Name, 
                Owner,
                [EnumDescription("Last Updated")]
                LastUpdated,
                Followers,
                Forks,
                [EnumDescription("Created Date")]
                CreatedOn, 
            };
        }

        public class Filter : FilterController
        {
            RepositoryController _parent;
            private EnumChoiceElement _orderby;
            private TrueFalseElement _ascendingElement;

            public Filter(RepositoryController parent)
            {
                _parent = parent;
            }

            public override void ViewDidLoad()
            {
                base.ViewDidLoad();

                //Load the root
                var root = new RootElement(Title) {
                    new Section("Order By") {
                        (_orderby = CreateEnumElement("Field", (int)_parent._filterModel.OrderBy, typeof(FilterModel.Order))),
                        (_ascendingElement = new TrueFalseElement("Ascending", _parent._filterModel.Ascending)),
                    },
                    new Section() {
                        new StyledElement("Save as Default", () => {  
                            Application.Account.RepoFilterObject = CreateFilterModel();
                            this.DismissModalViewControllerAnimated(true); 
                            this.ApplyFilter();
                        }, Images.Size) { Accessory = UITableViewCellAccessory.None },
                    }
                };

                Root = root;
            }

            private FilterModel CreateFilterModel()
            {
                var model = new FilterModel();
                model.OrderBy = _orderby.Obj;
                model.Ascending = _ascendingElement.Value;
                return model;
            }

            public override void ApplyFilter()
            {
                _parent._filterModel = CreateFilterModel();
                _parent.ApplyFilter();
            }

            public override void ViewWillAppear(bool animated)
            {
                base.ViewWillAppear(animated);
                TableView.ReloadData();
            }
        }
    }
}