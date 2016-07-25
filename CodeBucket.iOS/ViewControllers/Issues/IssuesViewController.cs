using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.TableViewSources;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssuesViewController : BaseViewController<IssuesViewModel>
    {
        private readonly Lazy<EnhancedTableView> _tableView =
            new Lazy<EnhancedTableView>(() => new EnhancedTableView(UITableViewStyle.Plain));

        public EnhancedTableView TableView => _tableView.Value;

        private IssueTableViewSource _tableViewSource;
        private IssueTableViewSource TableViewSource
        {
            get { return _tableViewSource; }
            set { this.RaiseAndSetIfChanged(ref _tableViewSource, value); }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.AddTableView(TableView);
            var searchBar = TableView.CreateSearchBar();

            var viewSegment = new UISegmentedControl(new[] { "Open", "Mine", "All" });
            var segmentBarButton = new UIBarButtonItem(viewSegment);
			segmentBarButton.Width = View.Frame.Width - 10f;

            ToolbarItems = new[]
            {
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace),
                segmentBarButton,
                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace)
            };
     
            var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var searchButton = new UIBarButtonItem(UIBarButtonSystemItem.Search);
            NavigationItem.RightBarButtonItems = new[] { addButton, searchButton };

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.Issues)
                    .Do(_ => TableView.Source?.Dispose())
                    .Where(x => x != null)
                    .Select(x => new IssueTableViewSource(TableView, x.Items))
                    .Do(x => TableViewSource = x)
                    .Subscribe(x => TableView.Source = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.TableViewSource)
                    .Subscribe(x => TableView.Source = x)
                    .AddTo(disposable);

                searchButton
                    .GetClickedObservable()
                    .Select(_ => ViewModel.Filter)
                    .Select(x => new IssuesFilterViewController(ViewModel.Username, ViewModel.Repository, x, y => ViewModel.Filter = y))
                    .Do(_ => ViewModel.SelectedFilter = -1)
                    .Subscribe(x => x.Present(this))
                    .AddTo(disposable);

                addButton
                    .GetClickedObservable()
                    .InvokeCommand(ViewModel.GoToNewIssueCommand)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.TableViewSource)
                    .Select(x => x.RequestMore)
                    .Switch()
                    .Where(x => ViewModel.Issues.HasMoreIssues)
                    .Subscribe(x => ViewModel.Issues.LoadMoreCommand.ExecuteIfCan())
                    .AddTo(disposable);

                viewSegment
                    .GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.SelectedFilter)
                    .Subscribe(x => viewSegment.SelectedSegment = x)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.Issues.SearchText)
                    .Subscribe(x => searchBar.Text = x)
                    .AddTo(disposable);

                searchBar.GetCanceledObservable()
                    .Subscribe(x => ViewModel.Issues.SearchText = null)
                    .AddTo(disposable);

                searchBar.GetChangedObservable()
                    .Subscribe(x => ViewModel.Issues.SearchText = x)
                    .AddTo(disposable);

                this.WhenAnyObservable(x => x.ViewModel.Issues.LoadMoreCommand.IsExecuting)
                    .StartWith(false)
                    .Subscribe(x => TableView.IsLoading = x)
                    .AddTo(disposable);
            });
        }

        protected override void Navigate(UIViewController viewController)
        {
            var issueAddViewController = viewController as IssueAddViewController;
            if (issueAddViewController != null)
                issueAddViewController.Present(this);
            else
                base.Navigate(viewController);
        }

        public override void ViewWillAppear(bool animated)
        {
			base.ViewWillAppear(animated);
            NavigationController.SetToolbarHidden(false, animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationController.SetToolbarHidden(true, animated);
        }
    }
}

