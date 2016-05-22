using System;
using CodeBucket.Core.ViewModels.Source;
using System.Reactive.Linq;
using UIKit;
using CodeBucket.TableViewSources;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Source
{
    public class SourceTreeViewController : BaseTableViewController<SourceTreeViewModel>
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var searchBar = TableView.CreateSearchBar();
            TableView.Source = new SourceTreeTableViewSource(TableView, ViewModel.Items);

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

