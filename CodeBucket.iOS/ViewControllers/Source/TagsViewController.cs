using System;
using CodeBucket.Core.ViewModels.Source;
using CodeBucket.TableViewSources;
using CodeBucket.Views;
using UIKit;

namespace CodeBucket.ViewControllers.Source
{
    public class TagsViewController : BaseTableViewController<TagsViewModel, GitReferenceItemViewModel>
    {
        public TagsViewController()
        {
        }

        public TagsViewController(string username, string repository)
        {
            ViewModel = new TagsViewModel(username, repository);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Devtoolstag.ToEmptyListImage(), "There are no tags."));
            TableView.Source = new ReferenceTableViewSource(TableView, ViewModel.Items);
        }
    }
}

