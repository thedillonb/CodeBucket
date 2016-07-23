using System.Linq;
using System.Collections.Generic;
using System;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Linq;
using CodeBucket.Client.V1;
using CodeBucket.Core.Filters;
using CodeBucket.Core.Messages;
using System.Reactive.Subjects;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssueListViewModel : ReactiveObject, IPaginatableViewModel, IListViewModel<IssueItemViewModel>
    {
        private readonly ISubject<Issue> _selectSubject = new Subject<Issue>();
        private readonly IssuesFilterModel _filter;
        private readonly IDisposable _issueAddDisposable, _issueDeleteDisposable, _issueUpdateDisposable;

        public IReadOnlyReactiveList<IssueItemViewModel> Items { get; }

        public IReactiveCommand<Unit> LoadMoreCommand { get; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set { this.RaiseAndSetIfChanged(ref _searchText, value); }
        }

        public IObservable<Issue> IssueSelected => _selectSubject.AsObservable();

        private readonly ObservableAsPropertyHelper<bool> _isEmpty;
        public bool IsEmpty => _isEmpty.Value;

        private bool _hasMoreIssues;
        public bool HasMoreIssues
        {
            get { return _hasMoreIssues; }
            set { this.RaiseAndSetIfChanged(ref _hasMoreIssues, value); }
        }

        private bool DoesIssueBelong(Issue model)
        {
            if (_filter == null)
                return true;

            if (_filter.Status != null && !_filter.Status.IsDefault())
                if (!PropertyToUrl(null, _filter.Status).Any(x => x.Item2.Equals(model.Status)))
                    return false;
            if (_filter.Kind != null && !_filter.Kind.IsDefault())
                if (!PropertyToUrl(null, _filter.Kind).Any(x => x.Item2.Equals(model.Metadata.Kind)))
                    return false;
            if (_filter.Priority != null && !_filter.Priority.IsDefault())
                if (!PropertyToUrl(null, _filter.Priority).Any(x => x.Item2.Equals(model.Priority)))
                    return false;
            if (!string.IsNullOrEmpty(_filter.AssignedTo))
                if (!object.Equals(_filter.AssignedTo, (model.Responsible == null ? "unassigned" : model.Responsible.Username)))
                    return false;
            if (!string.IsNullOrEmpty(_filter.ReportedBy))
                if (model.ReportedBy == null || !object.Equals(_filter.ReportedBy, model.ReportedBy.Username))
                    return false;
            if (!string.IsNullOrEmpty(_filter.Milestone))
                if (model?.Metadata?.Milestone == null || !object.Equals(_filter.Milestone, model.Metadata.Milestone))
                    return false;
            if (!string.IsNullOrEmpty(_filter.Version))
                if (model?.Metadata?.Version == null || !object.Equals(_filter.Version, model.Metadata.Version))
                    return false;
            if (!string.IsNullOrEmpty(_filter.Component))
                if (model?.Metadata?.Component == null || !object.Equals(_filter.Component, model.Metadata.Component))
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
                    ret.AddLast(Tuple.Create(name, objectName));
                }
            }
            return ret;
        }

        public IssueListViewModel(
            string username, string repository,
            IssuesFilterModel filter,
            IApplicationService applicationService = null,
            IMessageService messageService = null)
        {
            _filter = filter;
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            messageService = messageService ?? Locator.Current.GetService<IMessageService>();
            var currentUser = applicationService.Account.Username;

            var issues = new ReactiveList<Issue>(resetChangeThreshold: 10);
            Items = issues.CreateDerivedCollection(
                x =>
                {
                    var vm = new IssueItemViewModel(x);
                    vm.GoToCommand.Select(_ => x).Subscribe(_selectSubject);
                    return vm;
                },
                x => x.Title.ContainsKeyword(SearchText),
                signalReset: this.WhenAnyValue(x => x.SearchText));

            int startPage = 0;
            HasMoreIssues = true;

            LoadMoreCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.HasMoreIssues),
                async _ =>
            {
                var search = new LinkedList<Tuple<string, string>>();
                if (_filter != null)
                {
                    if (_filter.Status != null && !_filter.Status.IsDefault())
                        foreach (var a in PropertyToUrl("status", _filter.Status)) search.AddLast(a);
                    if (_filter.Kind != null && !_filter.Kind.IsDefault())
                        foreach (var a in PropertyToUrl("kind", _filter.Kind)) search.AddLast(a);
                    if (_filter.Priority != null && !_filter.Priority.IsDefault())
                        foreach (var a in PropertyToUrl("priority", _filter.Priority)) search.AddLast(a);
                    if (!string.IsNullOrEmpty(_filter.AssignedTo))
                    {
                        search.AddLast(Tuple.Create("responsible",
                            _filter.AssignedTo.Equals("unassigned") ? string.Empty : _filter.AssignedTo));
                    }
                    if (!string.IsNullOrEmpty(_filter.ReportedBy))
                        search.AddLast(Tuple.Create("reported_by", _filter.ReportedBy));

                    // For some reason, server side sorting by local_id returns ascending order.
                    // That's terrible.
                    if (_filter.OrderBy != IssuesFilterModel.Order.Local_Id)
                        search.AddLast(Tuple.Create("sort", _filter.OrderBy.ToString().ToLower()));

                    if (!string.IsNullOrEmpty(_filter.Milestone))
                        search.AddLast(Tuple.Create("milestone", _filter.Milestone));
                    if (!string.IsNullOrEmpty(_filter.Version))
                        search.AddLast(Tuple.Create("version", _filter.Version));
                    if (!string.IsNullOrEmpty(_filter.Component))
                        search.AddLast(Tuple.Create("component", _filter.Component));
                }

                var x = await applicationService.Client.Issues.GetAll(username, repository, startPage, search: search);
                startPage += x.Issues.Count;
                issues.AddRange(x.Issues);
                HasMoreIssues = !(startPage == x.Count);
            });

            LoadMoreCommand.IsExecuting.CombineLatest(issues.IsEmptyChanged, (x, y) => !x && y)
                .ToProperty(this, x => x.IsEmpty, out _isEmpty);

            _issueAddDisposable = messageService.Listen<IssueAddMessage>(x =>
            {
                if (DoesIssueBelong(x.Issue))
                    issues.Insert(0, x.Issue);
            });

            _issueDeleteDisposable = messageService.Listen<IssueDeleteMessage>(x =>
            {
                var deletedLocalId = x.Issue.LocalId;
                var deletedIssue = issues.FirstOrDefault(y => deletedLocalId == y.LocalId);
                if (deletedIssue != null)
                    issues.Remove(deletedIssue);
            });

            _issueUpdateDisposable = messageService.Listen<IssueUpdateMessage>(x =>
            {
                var localId = x.Issue.LocalId;
                var listIssue = issues.FirstOrDefault(y => localId == y.LocalId);
                if (listIssue != null)
                    issues[issues.IndexOf(listIssue)] = x.Issue;
            });
        }
    }
}

