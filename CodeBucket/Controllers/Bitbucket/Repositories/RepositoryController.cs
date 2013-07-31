using MonoTouch.Dialog;
using MonoTouch.UIKit;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System.Linq;
using CodeBucket.Bitbucket.Controllers;
using System;
using System.Drawing;
using CodeFramework.Controllers;
using CodeFramework.Elements;
using CodeFramework.Views;


namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryController : BaseModelDrivenController
    {
        FilterModel _filterModel = Application.Account.RepoFilterObject;
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }

        public new List<RepositoryDetailedModel> Model
        {
            get { return (List<RepositoryDetailedModel>)base.Model; }
            set { base.Model = value; }
        }

        public RepositoryController(string username, bool refresh = true)
            : base(typeof(List<RepositoryDetailedModel>), refresh: refresh)
        {
            Title = "Repositories";
            Username = username;
            ShowOwner = false;
            EnableFilter = true;
            Style = UITableViewStyle.Plain;
        }

        protected override object OnUpdateModel(bool forced)
        {
            var a = Application.Client.Users[Username].GetInfo(forced).Repositories;
            return a.OrderBy(x => x.Name).ToList();
        }


        static int[] _ceilings = FilterController.IntegerCeilings;
        private static string CreateRangeString(int key, IEnumerable<int> ranges)
        {
            return ranges.LastOrDefault(x => x < key) + " to " + (key - 1);
        }

        protected Element CreateElement(RepositoryDetailedModel repo)
        {
            var description = Application.Account.HideRepositoryDescriptionInList ? string.Empty : repo.Description;
            var sse = new RepositoryElement(repo.Name, repo.FollowersCount, repo.ForkCount, description, repo.Owner, new Uri(repo.LargeLogo(64))) { ShowOwner = ShowOwner };
            sse.Tapped += () => NavigationController.PushViewController(new RepositoryInfoController(repo), true);
            return sse;
        }

        private List<Section> CreateSection(IEnumerable<IGrouping<int, RepositoryDetailedModel>> results, string title, string prefix = null)
        {
            var sections = new List<Section>();
            InvokeOnMainThread(() => {
                foreach (var groups in results)
                {
                    var text = (prefix != null ? prefix + " " : "") + CreateRangeString(groups.Key, _ceilings) + " " + title;
                    var sec = new Section(new TableViewSectionView(text));
                    sections.Add(sec);
                    foreach (var y in groups)
                        sec.Add(CreateElement(y));
                }
            });
            return sections;
        }

        private List<Section> CreateSection(IEnumerable<IGrouping<string, RepositoryDetailedModel>> results)
        {
            var sections = new List<Section>();
            InvokeOnMainThread(() => {
                foreach (var groups in results)
                {
                    var sec = new Section(new TableViewSectionView(groups.Key));
                    sections.Add(sec);
                    foreach (var y in groups)
                        sec.Add(CreateElement(y));
                }
            });
            return sections;
        }

        protected override void OnRender()
        {
            if (Model == null)
                return;

            var model = Model as List<RepositoryDetailedModel>;

            var order = (FilterModel.Order)_filterModel.OrderBy;
            List<Section> sections = null;

            if (order == FilterModel.Order.Forks)
            {
                var a = model.OrderBy(x => x.ForkCount).GroupBy(x => _ceilings.First(r => r > x.ForkCount));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Forks");
            }
            else if (order == FilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => _ceilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Updated");
            }
            else if (order == FilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => _ceilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Created");
            }
            else if (order == FilterModel.Order.Followers)
            {
                var a = model.OrderBy(x => x.FollowersCount).GroupBy(x => _ceilings.First(r => r > x.FollowersCount));
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Followers");
            }
            else if (order == FilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner);
                a = _filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a);
            }
            else
            {
                var a = _filterModel.Ascending ? model.OrderBy(x => x.Name) : model.OrderByDescending(x => x.Name);
                sections = new List<Section>() { new Section() };
                foreach (var y in a)
                    sections[0].Add(CreateElement(y));
            }

            if (sections == null || sections.Count == 0)
                sections.Add(new Section() { new NoItemsElement("No Repositories") });

            InvokeOnMainThread(() => { Root = new RootElement(Title) { sections }; });
        }

        private void ApplyFilter()
        {
            Render();
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
                            this.DismissViewController(true, null); 
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