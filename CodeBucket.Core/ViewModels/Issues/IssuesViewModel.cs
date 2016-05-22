using System.Linq;
using BitbucketSharp.Models;
using System.Collections.Generic;
using System;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssuesViewModel : BaseViewModel, ILoadableViewModel, IListViewModel<IssueItemViewModel>
    {
        public IReadOnlyReactiveList<IssueItemViewModel> Items { get; }

        public IReactiveCommand<object> GoToNewIssueCommand { get; } = ReactiveCommand.Create();

        //public ICommand GoToFiltersCommand
        //{
        //    get { return new MvxCommand(() => ShowViewModel<IssuesFiltersViewModel>(new IssuesFiltersViewModel.NavObject { Username = Username, Repository = Repository })); }
        //}

        public IReactiveCommand<Unit> LoadCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        private int _selectedFilter;
        public int SelectedFilter
        {
            get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
        }

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
            Items = issues.CreateDerivedCollection(
                x =>
                {
                    var vm = new IssueItemViewModel(x);
                    vm.GoToCommand
                      .Select(_ => new IssueViewModel(username, repository, x))
                      .Subscribe(NavigateTo);
                    return vm;
                },
                x => x.Title.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

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


                var x = await applicationService.Client.Repositories.Issues.GetIssues(username, repository);
                issues.Reset(x.Issues);
            });

            LoadCommand.IsExecuting.CombineLatest(issues.IsEmptyChanged, (x, y) => !x && y)
                .ToProperty(this, x => x.IsEmpty, out _isEmpty);

            this.WhenAnyValue(x => x.SelectedFilter)
                .Skip(1)
                .InvokeCommand(LoadCommand);
        }
    }
}

