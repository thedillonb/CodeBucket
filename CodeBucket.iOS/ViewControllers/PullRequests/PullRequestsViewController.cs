using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.TableViewCells;
using System;
using CodeBucket.Views;
using ReactiveUI;
using System.Linq;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestsViewController : ViewModelDrivenDialogViewController<PullRequestsViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            EnableSearch = true;

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolspullrequest.ToImage(64f), "There are no pull requests."));

            var viewSegment = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            NavigationItem.TitleView = viewSegment;

            TableView.RegisterNibForCellReuse(PullRequestCellView.Nib, PullRequestCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

            var section = new Section();
            Root.Reset(section);

            ViewModel
                .PullRequests
                .ChangedObservable()
                .Subscribe(x => section.Reset(x.Select(y => new PullRequestElement(y))));

            OnActivation(d =>
            {
                d(viewSegment.GetChangedObservable().Subscribe(x => ViewModel.SelectedFilter = (BitbucketSharp.Models.V2.PullRequestState)x));
                d(ViewModel.WhenAnyValue(x => x.SelectedFilter).Subscribe(x => viewSegment.SelectedSegment = (int)x));
            });

        }
    }
}

