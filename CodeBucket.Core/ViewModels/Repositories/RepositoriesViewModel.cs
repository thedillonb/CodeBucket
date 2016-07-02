using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using CodeBucket.Core.Filters;
using CodeBucket.Client.Models;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.ViewModels.Repositories
{
    public abstract class RepositoriesViewModel : LoadableViewModel
    {
		private readonly FilterableCollectionViewModel<Repository, RepositoriesFilterModel> _repositories;

        public bool ShowRepositoryDescription
        {
			get { return this.GetApplication().Account.RepositoryDescriptionInList; }
        }

        public FilterableCollectionViewModel<Repository, RepositoriesFilterModel> Repositories
        {
            get { return _repositories; }
        }

        public ICommand GoToRepositoryCommand
        {
            get
            {
                return new MvxCommand<Repository>(x =>
                {
                    var r = new RepositoryIdentifier(x.FullName);
                    this.ShowViewModel<RepositoryViewModel>(new RepositoryViewModel.NavObject { Username = r?.Owner, RepositorySlug = r?.Name });
                });
            }
        }

        protected RepositoriesViewModel(string filterKey = "RepositoryController")
        {
			_repositories = new FilterableCollectionViewModel<Repository, RepositoriesFilterModel>(filterKey);
			_repositories.FilteringFunction = x => Repositories.Filter.Ascending ? x.OrderBy(y => y.Name) : x.OrderByDescending(y => y.Name);
            _repositories.GroupingFunction = CreateGroupedItems;
            _repositories.Bind(x => x.Filter).Subscribe(x => Repositories.Refresh());
        }

		private IEnumerable<IGrouping<string, Repository>> CreateGroupedItems(IEnumerable<Repository> model)
        {
            var order = Repositories.Filter.OrderBy;

            if (order == RepositoriesFilterModel.Order.LastUpdated)
            {
                var a = model.OrderByDescending(x => x.UpdatedOn).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UpdatedOn.TotalDaysAgo()));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Days Ago", "Updated");
            }
            if (order == RepositoriesFilterModel.Order.CreatedOn)
            {
                var a = model.OrderByDescending(x => x.CreatedOn).GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.CreatedOn.TotalDaysAgo()));
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return FilterGroup.CreateNumberedGroup(a, "Days Ago", "Created");
            }
            if (order == RepositoriesFilterModel.Order.Owner)
            {
                var a = model.OrderBy(x => x.Name).GroupBy(x => x.Owner.Username);
                a = Repositories.Filter.Ascending ? a.OrderBy(x => x.Key) : a.OrderByDescending(x => x.Key);
                return a.ToList();
            }

            return null;
        }
    }
}