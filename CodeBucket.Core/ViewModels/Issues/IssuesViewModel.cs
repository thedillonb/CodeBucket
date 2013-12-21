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

namespace CodeBucket.Core.ViewModels.Issues
{
	public class IssuesViewModel : LoadableViewModel
    {
		private MvxSubscriptionToken _addToken, _editToken;

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

		public void Init(NavObject nav)
		{
			Username = nav.Username;
			Repository = nav.Repository;
			_issues = new FilterableCollectionViewModel<IssueModel, IssuesFilterModel>("IssuesViewModel:" + Username + "/" + Repository);
			//_issues.GroupingFunction = Group;
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
		}

//
//		public ICommand GoToIssueCommand
//		{
//			get 
//			{ 
//				return new MvxCommand<IssueModel>(x =>
//					{
//						var isPullRequest = x.PullRequest != null && !(string.IsNullOrEmpty(x.PullRequest.HtmlUrl));
//						var s1 = x.Url.Substring(x.Url.IndexOf("/repos/") + 7);
//						var repoId = new RepositoryIdentifier(s1.Substring(0, s1.IndexOf("/issues")));
//
//						if (isPullRequest)
//							ShowViewModel<PullRequestViewModel>(new PullRequestViewModel.NavObject { Username = repoId.Owner, Repository = repoId.Name, Id = x.Number });
//						else
//							ShowViewModel<IssueViewModel>(new IssueViewModel.NavObject { Username = repoId.Owner, Repository = repoId.Name, Id = x.Number });
//					});
//			}
//		}
//
//		protected List<IGrouping<string, IssueModel>> Group(IEnumerable<IssueModel> model)
//		{
//			var order = Issues.Filter.SortType;
//			if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Comments)
//			{
//				var a = Issues.Filter.Ascending ? model.OrderBy(x => x.Comments) : model.OrderByDescending(x => x.Comments);
//				var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.Comments)).ToList();
//				return FilterGroup.CreateNumberedGroup(g, "Comments");
//			}
//			if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Updated)
//			{
//				var a = Issues.Filter.Ascending ? model.OrderBy(x => x.UpdatedAt) : model.OrderByDescending(x => x.UpdatedAt);
//				var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UpdatedAt.TotalDaysAgo()));
//				return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Updated");
//			}
//			if (order == BaseIssuesFilterModel<TFilterModel>.Sort.Created)
//			{
//				var a = Issues.Filter.Ascending ? model.OrderBy(x => x.CreatedAt) : model.OrderByDescending(x => x.CreatedAt);
//				var g = a.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.CreatedAt.TotalDaysAgo()));
//				return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Created");
//			}
//
//			return null;
//		}

        protected override Task Load(bool forceCacheInvalidation)
        {
//            string direction = _issues.Filter.Ascending ? "asc" : "desc";
//            string state = _issues.Filter.Open ? "open" : "closed";
//            string sort = _issues.Filter.SortType == IssuesFilterModel.Sort.None ? null : _issues.Filter.SortType.ToString().ToLower();
//            string labels = string.IsNullOrEmpty(_issues.Filter.Labels) ? null : _issues.Filter.Labels;
//            string assignee = string.IsNullOrEmpty(_issues.Filter.Assignee) ? null : _issues.Filter.Assignee;
//            string creator = string.IsNullOrEmpty(_issues.Filter.Creator) ? null : _issues.Filter.Creator;
//            string mentioned = string.IsNullOrEmpty(_issues.Filter.Mentioned) ? null : _issues.Filter.Mentioned;
//            string milestone = _issues.Filter.Milestone == null ? null : _issues.Filter.Milestone.Value;
//
//			var request = this.GetApplication().Client.Users[Username].Repositories[Repository].Issues.GetAll(sort: sort, labels: labels, state: state, direction: direction, 
//                                                                                          assignee: assignee, creator: creator, mentioned: mentioned, milestone: milestone);
//            return Issues.SimpleCollectionLoad(request, forceCacheInvalidation);
			throw new NotImplementedException();

        }


        private bool DoesIssueBelong(IssueModel model)
        {
			var filter = Issues.Filter;
			if (filter == null)
                return true;

			if (filter.Status != null && !filter.Status.IsDefault())
				if (!FieldToUrl(null, filter.Status).Any(x => x.Item2.Equals(model.Status)))
                    return false;
			if (filter.Kind != null && !filter.Kind.IsDefault())
				if (!FieldToUrl(null, filter.Kind).Any(x => x.Item2.Equals(model.Metadata.Kind)))
                    return false;
			if (filter.Priority != null && !filter.Priority.IsDefault())
				if (!FieldToUrl(null, filter.Priority).Any(x => x.Item2.Equals(model.Priority)))
                    return false;
			if (!string.IsNullOrEmpty(filter.AssignedTo))
			if (!object.Equals(filter.AssignedTo, (model.Responsible == null ? "unassigned" : model.Responsible.Username)))
                    return false;
			if (!string.IsNullOrEmpty(filter.ReportedBy))
				if (model.ReportedBy == null || !object.Equals(filter.ReportedBy, model.ReportedBy.Username))
                    return false;

            return true;
        }

        private static IEnumerable<Tuple<string, string>> FieldToUrl(string name, object o)
        {
            var ret = new LinkedList<Tuple<string, string>>();
            foreach (var f in o.GetType().GetFields())
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

