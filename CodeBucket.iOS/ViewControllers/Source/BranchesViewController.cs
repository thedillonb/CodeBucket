using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewSources;
using ReactiveUI;
using UIKit;

namespace CodeBucket.ViewControllers.Source
{
    public class BranchesViewController : BaseTableViewController<BranchesViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var searchBar = TableView.CreateSearchBar();
            TableView.Source = new ReferenceTableViewSource(TableView, ViewModel.Items);

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.SearchText)
                    .Subscribe(x => searchBar.Text = x)
                    .AddTo(disposable);

                searchBar.GetChangedObservable()
                    .Subscribe(x => ViewModel.SearchText = x)
                    .AddTo(disposable);
            });
        }
    }
}

