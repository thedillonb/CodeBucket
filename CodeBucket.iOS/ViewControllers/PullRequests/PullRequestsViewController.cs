using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using System;
using CodeBucket.Views;
using ReactiveUI;
using CodeBucket.TableViewSources;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestsViewController : BaseViewController<PullRequestsViewModel>
    {
        private readonly Lazy<EnhancedTableView> _tableView =
            new Lazy<EnhancedTableView>(() => new EnhancedTableView(UITableViewStyle.Plain));

        public EnhancedTableView TableView => _tableView.Value;

        private PullRequestTableViewSource _tableViewSource;
        private PullRequestTableViewSource TableViewSource
        {
            get { return _tableViewSource; }
            set { this.RaiseAndSetIfChanged(ref _tableViewSource, value); }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.AddTableView(TableView);
            var searchBar = TableView.CreateSearchBar();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolspullrequest.ToImage(64f), "There are no pull requests."));

            var viewSegment = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            NavigationItem.TitleView = viewSegment;

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.PullRequests)
                    .Do(_ => TableView.Source?.Dispose())
                    .Where(x => x != null)
                    .Select(x => new PullRequestTableViewSource(TableView, x.Items))
                    .Do(x => TableViewSource = x)
                    .Subscribe(x => TableView.Source = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.TableViewSource)
                    .Select(x => x.RequestMore)
                    .Switch()
                    .Where(x => ViewModel.PullRequests.HasMoreItems)
                    .Subscribe(x => ViewModel.PullRequests.LoadMoreCommand.ExecuteIfCan())
                    .AddTo(disposable);
                
                viewSegment.GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = (Client.PullRequestState)x)
                    .AddTo(disposable);

                ViewModel.WhenAnyValue(x => x.SelectedFilter)
                    .Subscribe(x => viewSegment.SelectedSegment = (int)x)
                    .AddTo(disposable);

                searchBar.GetCanceledObservable()
                    .Subscribe(x => ViewModel.PullRequests.SearchText = null)
                    .AddTo(disposable);

                searchBar.GetChangedObservable()
                    .Subscribe(x => ViewModel.PullRequests.SearchText = x)
                    .AddTo(disposable);

                this.WhenAnyObservable(x => x.ViewModel.PullRequests.LoadMoreCommand.IsExecuting)
                    .StartWith(false)
                    .Subscribe(x => TableView.IsLoading = x)
                    .AddTo(disposable);
            });
        }
    }
}

