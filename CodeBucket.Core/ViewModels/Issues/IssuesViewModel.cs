using System.Linq;
using System;
using CodeBucket.Core.Services;
using Splat;
using ReactiveUI;
using System.Reactive.Linq;
using CodeBucket.Core.Filters;

namespace CodeBucket.Core.ViewModels.Issues
{
    public class IssuesViewModel : BaseViewModel
    {
        private IssueListViewModel _listViewModel;
        public IssueListViewModel Issues
        {
            get { return _listViewModel; }
            private set { this.RaiseAndSetIfChanged(ref _listViewModel, value); }
        }

        public IReactiveCommand<object> GoToNewIssueCommand { get; } = ReactiveCommand.Create();

        private int _selectedFilter;
        public int SelectedFilter
        {
            get { return _selectedFilter; }
            set { this.RaiseAndSetIfChanged(ref _selectedFilter, value); }
        }

        private IssuesFilterModel _filter = IssuesFilterModel.CreateAllFilter();
        public IssuesFilterModel Filter
        {
            get { return _filter; }
            set { this.RaiseAndSetIfChanged(ref _filter, value); }
        }

        public IssuesViewModel(
            string username, string repository,
            IApplicationService applicationService = null,
            IMessageService messageService = null)
        {
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            messageService = messageService ?? Locator.Current.GetService<IMessageService>();
            var currentUser = applicationService.Account.Username;

            GoToNewIssueCommand
                .Select(_ => new IssueAddViewModel(username, repository))
                .Subscribe(NavigateTo);

            Title = "Issues";

            this.WhenAnyValue(x => x.SelectedFilter)
                .Subscribe(x =>
                {
                    if (x == 0)
                        Filter = IssuesFilterModel.CreateAllFilter();
                    else if (x == 1)
                        Filter = IssuesFilterModel.CreateOpenFilter();
                    else if (x == 2)
                        Filter = IssuesFilterModel.CreateMineFilter(currentUser);
                });

            this.WhenAnyValue(x => x.Filter)
                .Select(x => new IssueListViewModel(username, repository, x))
                .Do(x => x.LoadMoreCommand.ExecuteIfCan())
                .Subscribe(x => Issues = x);

            this.WhenAnyValue(x => x.Issues)
                .Select(x => x.IssueSelected)
                .Switch()
                .Select(x => new IssueViewModel(username, repository, x))
                .Subscribe(NavigateTo);
        }
    }
}

