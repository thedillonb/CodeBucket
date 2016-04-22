using CodeBucket.Core.ViewModels.PullRequests;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.TableViewCells;
using System;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.PullRequests
{
    public class PullRequestsViewController : ViewModelCollectionDrivenDialogViewController
    {
		private readonly UISegmentedControl _viewSegment;
 
		public PullRequestsViewController()
		{
			Title = "Pull Requests";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolspullrequest.ToImage(64f), "There are no pull requests.")); 

			_viewSegment = new UISegmentedControl(new object[] { "Open", "Merged", "Declined" });
            NavigationItem.TitleView = _viewSegment;
		}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.RegisterNibForCellReuse(PullRequestCellView.Nib, PullRequestCellView.Key);
            TableView.RowHeight = UITableView.AutomaticDimension;
            TableView.EstimatedRowHeight = 80f;

			var vm = (PullRequestsViewModel)ViewModel;
            BindCollection(vm.PullRequests, s => new PullRequestElement(s));

            OnActivation(d =>
            {
                d(_viewSegment.GetChangedObservable().Subscribe(x => vm.SelectedFilter = (BitbucketSharp.Models.V2.PullRequestState)x));
                d(vm.Bind(x => x.SelectedFilter, true).Subscribe(x => _viewSegment.SelectedSegment = (int)x));
            });

        }
    }
}

