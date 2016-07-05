using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using System;
using CodeBucket.Views;
using ReactiveUI;
using CodeBucket.TableViewSources;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestsViewController : BaseTableViewController<PullRequestsViewModel, PullRequestItemViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var source = new PullRequestTableViewSource(TableView, ViewModel.Items);
            TableView.Source = source;
            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolspullrequest.ToImage(64f), "There are no pull requests."));

            var viewSegment = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            NavigationItem.TitleView = viewSegment;

            OnActivation(disposable =>
            {
                viewSegment.GetChangedObservable()
                    .Subscribe(x => ViewModel.SelectedFilter = (Client.PullRequestState)x)
                    .AddTo(disposable);

                ViewModel.WhenAnyValue(x => x.SelectedFilter)
                    .Subscribe(x => viewSegment.SelectedSegment = (int)x)
                    .AddTo(disposable);
            });
        }
    }
}

