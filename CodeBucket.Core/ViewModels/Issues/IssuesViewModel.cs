using System.Threading.Tasks;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Filters;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Messages;
using Cirrious.MvvmCross.Plugins.Messenger;
using System.Linq;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeFramework.Core.Utils;

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssuesViewModel : LoadableViewModel
    {
        private MvxSubscriptionToken _addToken, _editToken, _deleteToken, _filterToken;

        public string Username { get; private set; }

        public string Repository { get; private set; }


		protected FilterableCollectionViewModel<IssueModel, IssuesFilterModel> _issues;
		public FilterableCollectionViewModel<IssueModel, IssuesFilterModel> Issues
		{
			get { return _issues; }
		}

		public ICommand GoToNewIssueCommand
		{
			get { return new MvxCommand(() => ShowViewModel<IssueAddViewModel>(new IssueAddViewModel.NavObject { Username = Username, Repository = Repository })); }
		}

        public ICommand GoToFiltersCommand
        {
            get { return new MvxCommand(() => ShowViewModel<IssuesFiltersViewModel>(new IssuesFiltersViewModel.NavObject { Username = Username, Repository = Repository })); }
        }

		public void Init(NavObject nav)
		{
			Username = nav.Username;
			Repository = nav.Repository;
			_issues = new FilterableCollectionViewModel<IssueModel, IssuesFilterModel>("IssuesViewModel:" + Username + "/" + Repository);
			_issues.GroupingFunction = Group;
			_issues.Bind(x => x.Filter, () => LoadCommand.Execute(true));

			_addToken = Messenger.SubscribeOnMainThread<IssueAddMessage>(x =>
			{
				if (x.Issue == null || !DoesIssueBelong(x.Issue))
					return;
				Issues.Items.Insert(0, x.Issue);
			});

			_editToken = Messenger.SubscribeOnMainThread<IssueEditMessage>(x =>
			{
				if (x.Issue == null || !DoesIssueBelong(x.Issue))
					return;
				
				var item = Issues.Items.FirstOrDefault(y => y.LocalId == x.Issue.LocalId);
				if (item == null)
					return;

				var index = Issues.Items.IndexOf(item);

				using (Issues.DeferRefresh())
				{
					Issues.Items.RemoveAt(index);
					Issues.Items.Insert(index, x.Issue);
				}
			});

            _deleteToken = Messenger.SubscribeOnMainThread<IssueDeleteMessage>(x =>
            {
                var find = Issues.Items.FirstOrDefault(i => i.LocalId == x.Issue.LocalId);
                if (find != null)
                    Issues.Items.Remove(find);
            });

            _filterToken = Messenger.SubscribeOnMainThread<IssuesFilterMessage>(x => {
                _issues.Filter = x.Filter;
            });
		}


		public ICommand GoToIssueCommand
		{
			get 
			{ 
				return new MvxCommand<IssueModel>(x => ShowViewModel<IssueViewModel>(new IssueViewModel.NavObject { Username = Username, Repository = Repository, Id = x.LocalId }));
			}
		}

		protected List<IGrouping<string, IssueModel>> Group(IEnumerable<IssueModel> model)
		{
			var order = Issues.Filter.OrderBy;
			if (order == IssuesFilterModel.Order.Status)
			{
				return model.GroupBy(x => x.Status).ToList();
			}
			if (order == IssuesFilterModel.Order.Component)
			{
				return model.GroupBy(x => x.Metadata != null ? x.Metadata.Component : "N/A").ToList();
			}
			if (order == IssuesFilterModel.Order.Milestone)
			{
				return model.GroupBy(x => x.Metadata != null ? x.Metadata.Milestone : "N/A").ToList();
			}
			if (order == IssuesFilterModel.Order.Version)
			{
				return model.GroupBy(x => x.Metadata != null ? x.Metadata.Version : "N/A").ToList();
			}
			if (order == IssuesFilterModel.Order.Utc_Last_Updated)
			{
				var g = model.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
				return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Updated");
			}
			if (order == IssuesFilterModel.Order.Created_On)
			{
				var g = model.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
				return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Created");
			}
			if (order == IssuesFilterModel.Order.Priority)
			{
				return model.GroupBy(x => x.Priority).ToList();
			}

			return null;
		}

        protected override Task Load(bool forceCacheInvalidation)
        {

            LinkedList<Tuple<string, string>> filter = new LinkedList<Tuple<string, string>>();
			var issuesFilter = Issues.Filter;
			if (issuesFilter != null)
            {
				if (issuesFilter.Status != null && !issuesFilter.Status.IsDefault())
                    foreach (var a in PropertyToUrl("status", issuesFilter.Status)) filter.AddLast(a);
				if (issuesFilter.Kind != null && !issuesFilter.Kind.IsDefault())
                    foreach (var a in PropertyToUrl("kind", issuesFilter.Kind)) filter.AddLast(a);
				if (issuesFilter.Priority != null && !issuesFilter.Priority.IsDefault())
                    foreach (var a in PropertyToUrl("priority", issuesFilter.Priority)) filter.AddLast(a);
				if (!string.IsNullOrEmpty(issuesFilter.AssignedTo))
                {
					if (issuesFilter.AssignedTo.Equals("unassigned"))
                        filter.AddLast(new Tuple<string, string>("responsible", ""));
                    else
						filter.AddLast(new Tuple<string, string>("responsible", issuesFilter.AssignedTo));
                }
				if (!string.IsNullOrEmpty(issuesFilter.ReportedBy))
					filter.AddLast(new Tuple<string, string>("reported_by", issuesFilter.ReportedBy));

				filter.AddLast(new Tuple<string, string>("sort", ((IssuesFilterModel.Order)issuesFilter.OrderBy).ToString().ToLower()));
            }

			return Issues.SimpleCollectionLoad(() => this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.GetIssues(0, 50, filter).Issues);

        }


        private bool DoesIssueBelong(IssueModel model)
        {
			var filter = Issues.Filter;
			if (filter == null)
                return true;

			if (filter.Status != null && !filter.Status.IsDefault())
                if (!PropertyToUrl(null, filter.Status).Any(x => x.Item2.Equals(model.Status)))
                    return false;
			if (filter.Kind != null && !filter.Kind.IsDefault())
                if (!PropertyToUrl(null, filter.Kind).Any(x => x.Item2.Equals(model.Metadata.Kind)))
                    return false;
			if (filter.Priority != null && !filter.Priority.IsDefault())
                if (!PropertyToUrl(null, filter.Priority).Any(x => x.Item2.Equals(model.Priority)))
                    return false;
			if (!string.IsNullOrEmpty(filter.AssignedTo))
			if (!object.Equals(filter.AssignedTo, (model.Responsible == null ? "unassigned" : model.Responsible.Username)))
                    return false;
			if (!string.IsNullOrEmpty(filter.ReportedBy))
				if (model.ReportedBy == null || !object.Equals(filter.ReportedBy, model.ReportedBy.Username))
                    return false;

            return true;
        }

        private static IEnumerable<Tuple<string, string>> PropertyToUrl(string name, object o)
        {
            var ret = new LinkedList<Tuple<string, string>>();
            foreach (var f in o.GetType().GetProperties())
            {
                if ((bool)f.GetValue(o))
                {
                    //Special case for "on hold"
                    var objectName = f.Name.ToLower();
                    if (objectName.Equals("onhold"))
                        objectName = "on hold";
                    ret.AddLast(new Tuple<string, string>(name, objectName));
                }
            }
            return ret;
        }

        public void CreateIssue(IssueModel issue)
        {
            if (!DoesIssueBelong(issue))
                return;
            Issues.Items.Add(issue);
        }

		public class NavObject
		{
			public string Username { get; set; }
			public string Repository { get; set; }
		}
    }
}

