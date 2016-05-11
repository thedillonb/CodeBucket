using System.Threading.Tasks;
using CodeBucket.Core.Filters;
using System.Windows.Input;
using CodeBucket.Core.Messages;
using System.Linq;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeBucket.Core.Utils;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssuesViewModel : BaseViewModel, ILoadableViewModel
    {
        public bool Simple { get; private set; }

        public IReadOnlyReactiveList<IssueItemViewModel> Issues { get; }

        public IReactiveCommand<object> GoToNewIssueCommand { get; }

        //public ICommand GoToFiltersCommand
        //{
        //    get { return new MvxCommand(() => ShowViewModel<IssuesFiltersViewModel>(new IssuesFiltersViewModel.NavObject { Username = Username, Repository = Repository })); }
        //}

        public IReactiveCommand<Unit> LoadCommand { get; }

  //      protected List<IGrouping<string, IssueModel>> Group(IEnumerable<IssueModel> model)
		//{
		//	var order = Issues.Filter.OrderBy;
		//	if (order == IssuesFilterModel.Order.Status)
		//	{
		//		return model.GroupBy(x => x.Status).ToList();
		//	}
		//	if (order == IssuesFilterModel.Order.Component)
		//	{
		//		return model.GroupBy(x => x.Metadata != null ? x.Metadata.Component : "N/A").ToList();
		//	}
		//	if (order == IssuesFilterModel.Order.Milestone)
		//	{
		//		return model.GroupBy(x => x.Metadata != null ? x.Metadata.Milestone : "N/A").ToList();
		//	}
		//	if (order == IssuesFilterModel.Order.Version)
		//	{
		//		return model.GroupBy(x => x.Metadata != null ? x.Metadata.Version : "N/A").ToList();
		//	}
		//	if (order == IssuesFilterModel.Order.Utc_Last_Updated)
		//	{
		//		var g = model.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UtcLastUpdated.TotalDaysAgo()));
		//		return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Updated");
		//	}
		//	if (order == IssuesFilterModel.Order.Created_On)
		//	{
		//		var g = model.GroupBy(x => FilterGroup.IntegerCeilings.First(r => r > x.UtcCreatedOn.TotalDaysAgo()));
		//		return FilterGroup.CreateNumberedGroup(g, "Days Ago", "Created");
		//	}
		//	if (order == IssuesFilterModel.Order.Priority)
		//	{
		//		return model.GroupBy(x => x.Priority).ToList();
		//	}

		//	return null;
		//}

   //     private bool DoesIssueBelong(IssueModel model)
   //     {
			//var filter = Issues.Filter;
			//if (filter == null)
   //             return true;

			//if (filter.Status != null && !filter.Status.IsDefault())
   //             if (!PropertyToUrl(null, filter.Status).Any(x => x.Item2.Equals(model.Status)))
   //                 return false;
			//if (filter.Kind != null && !filter.Kind.IsDefault())
   //             if (!PropertyToUrl(null, filter.Kind).Any(x => x.Item2.Equals(model.Metadata.Kind)))
   //                 return false;
			//if (filter.Priority != null && !filter.Priority.IsDefault())
   //             if (!PropertyToUrl(null, filter.Priority).Any(x => x.Item2.Equals(model.Priority)))
   //                 return false;
			//if (!string.IsNullOrEmpty(filter.AssignedTo))
			//if (!object.Equals(filter.AssignedTo, (model.Responsible == null ? "unassigned" : model.Responsible.Username)))
   //                 return false;
			//if (!string.IsNullOrEmpty(filter.ReportedBy))
			//	if (model.ReportedBy == null || !object.Equals(filter.ReportedBy, model.ReportedBy.Username))
   //                 return false;

   //         return true;
   //     }

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

        public IssuesViewModel(
            string username, string repository,
            IApplicationService applicationService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            GoToNewIssueCommand
                .Select(_ => new IssueAddViewModel(username, repository))
                .Subscribe(NavigateTo);

            Title = "Issues";

            var issues = new ReactiveList<IssueModel>();

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                LinkedList<Tuple<string, string>> filter = new LinkedList<Tuple<string, string>>();
                //var issuesFilter = Issues.Filter;
                //if (issuesFilter != null)
                //{
                //    if (issuesFilter.Status != null && !issuesFilter.Status.IsDefault())
                //        foreach (var a in PropertyToUrl("status", issuesFilter.Status)) filter.AddLast(a);
                //    if (issuesFilter.Kind != null && !issuesFilter.Kind.IsDefault())
                //        foreach (var a in PropertyToUrl("kind", issuesFilter.Kind)) filter.AddLast(a);
                //    if (issuesFilter.Priority != null && !issuesFilter.Priority.IsDefault())
                //        foreach (var a in PropertyToUrl("priority", issuesFilter.Priority)) filter.AddLast(a);
                //    if (!string.IsNullOrEmpty(issuesFilter.AssignedTo))
                //    {
                //        if (issuesFilter.AssignedTo.Equals("unassigned"))
                //            filter.AddLast(new Tuple<string, string>("responsible", ""));
                //        else
                //            filter.AddLast(new Tuple<string, string>("responsible", issuesFilter.AssignedTo));
                //    }
                //    if (!string.IsNullOrEmpty(issuesFilter.ReportedBy))
                //        filter.AddLast(new Tuple<string, string>("reported_by", issuesFilter.ReportedBy));

                //    //filter.AddLast(new Tuple<string, string>("sort", ((IssuesFilterModel.Order)issuesFilter.OrderBy).ToString().ToLower()));
                //}


                await applicationService.Client.Repositories.Issues.GetIssues(username, repository);
            });
        }
    }
}

