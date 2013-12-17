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
using CodeFramework.Filters.Models;


namespace CodeBucket.Controllers
{
    public class RepositoriesController : ListController<RepositoryDetailedModel, RepositoriesFilterModel>
    {
        public string Username { get; private set; }
        private readonly string _filterKey;

        public RepositoriesController(IListView<RepositoryDetailedModel> view, string username, string filterKey = "RepositoryController")
            : base(view)
        {
            Username = username;
            _filterKey = filterKey;

            Filter = Application.Account.GetFilter<RepositoriesFilterModel>(_filterKey);
        }

        public override void Update(bool force)
        {
            Model = new ListModel<RepositoryDetailedModel> {
                Data = Application.Client.Users[Username].GetInfo(force).Repositories.OrderBy(x => x.Name).ToList()
            };
        }

        protected override List<RepositoryDetailedModel> FilterModel(List<RepositoryDetailedModel> model, RepositoriesFilterModel filter)
        {
            return (Filter.Ascending ? model.OrderBy(x => x.Name) : model.OrderByDescending(x => x.Name)).ToList();
        }

        protected override List<IGrouping<string, RepositoryDetailedModel>> GroupModel(List<RepositoryDetailedModel> model, RepositoriesFilterModel filter)
        {
            var order = (RepositoriesFilterModel.Order)Filter.OrderBy;
            if (order == RepositoriesFilterModel.Order.Forks)
            {
                var a = model.OrderBy(x => x.ForkCount).GroupBy(x => IntegerCeilings.First(r => r > x.ForkCount));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Forks");
            }
            else if (order == RepositoriesFilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UtcLastUpdated).GroupBy(x => IntegerCeilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Days Ago", "Updated");
            }
            else if (order == RepositoriesFilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.UtcCreatedOn).GroupBy(x => IntegerCeilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Days Ago", "Created");
            }
            else if (order == RepositoriesFilterModel.Order.Followers)
            {
                var a = model.OrderBy(x => x.FollowersCount).GroupBy(x => IntegerCeilings.First(r => r > x.FollowersCount));
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return CreateNumberedGroup(a, "Followers");
            }
            else if (order == RepositoriesFilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner);
                a = Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return a.ToList();
            }

            return null;
        }

        protected override void SaveFilterAsDefault(RepositoriesFilterModel filter)
        {
            Application.Account.AddFilter(_filterKey, filter);
        }


    }
}