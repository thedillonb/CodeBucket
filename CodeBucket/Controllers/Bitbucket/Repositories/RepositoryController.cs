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
using CodeFramework.Filters.Controllers;
using CodeBucket.Filters.Models;


namespace CodeBucket.Bitbucket.Controllers.Repositories
{
    public class RepositoryController : BaseModelDrivenController
    {
        public string Username { get; private set; }
        public bool ShowOwner { get; set; }
        private readonly string _filterKey;

        public new List<RepositoryDetailedModel> Model
        {
            get { return (List<RepositoryDetailedModel>)base.Model; }
            set { base.Model = value; }
        }

        public RepositoryController(string username, bool refresh = true, string filterKey = "RepositoryController")
            : base(refresh: refresh)
        {
            Username = username;
            ShowOwner = false;
            EnableFilter = true;
            Style = UITableViewStyle.Plain;
            Title = "Repositories".t();
            SearchPlaceholder = "Search Repositories".t();
            _filterKey = filterKey;

            var filter = Application.Account.GetFilter(_filterKey);
            SetFilterModel(filter != null ? filter.GetData<RepositoriesFilterModel>() : new RepositoriesFilterModel());
        }

        protected override object OnUpdateModel(bool forced)
        {
            return Application.Client.Users[Username].GetInfo(forced).Repositories.OrderBy(x => x.Name).ToList();
        }

        protected override void SaveFilterAsDefault(CodeFramework.Filters.Models.FilterModel model)
        {
            Application.Account.AddFilter(_filterKey, model);
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
            var model = Model;
            if (model == null)
                return;

            var filterModel = GetFilterModel<RepositoriesFilterModel>();

            var order = (RepositoriesFilterModel.Order)filterModel.OrderBy;
            List<Section> sections = null;

            if (order == RepositoriesFilterModel.Order.Forks)
            {
                var a = model.OrderBy(x => x.ForkCount).GroupBy(x => _ceilings.First(r => r > x.ForkCount));
                a = filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Forks");
            }
            else if (order == RepositoriesFilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => _ceilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                a = filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Updated");
            }
            else if (order == RepositoriesFilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => _ceilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                a = filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Days Ago", "Created");
            }
            else if (order == RepositoriesFilterModel.Order.Followers)
            {
                var a = model.OrderBy(x => x.FollowersCount).GroupBy(x => _ceilings.First(r => r > x.FollowersCount));
                a = filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a, "Followers");
            }
            else if (order == RepositoriesFilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner);
                a = filterModel.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                sections = CreateSection(a);
            }
            else
            {
                var a = filterModel.Ascending ? model.OrderBy(x => x.Name) : model.OrderByDescending(x => x.Name);
                sections = new List<Section>() { new Section() };
                foreach (var y in a)
                    sections[0].Add(CreateElement(y));
            }

            //Could have easily done a check for model.Count == 0. However, doing this here allows me to do filtering at some point
            //and use the same logic. If I did model.Count that wouldn't take into account the items I filtered out.
            if (sections == null || sections.Count == 0 || sections.Count(x => x.Elements.Count > 0) == 0)
                sections.Add(new Section() { new NoItemsElement("No Repositories") });

            Root = new RootElement(Title) { sections };
        }

        protected override FilterController CreateFilterController()
        {
            return new CodeBucket.Filters.Controllers.RepositoriesFilterController();
        }
    }
}