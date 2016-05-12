using CodeBucket.Core.ViewModels.Groups;
using System;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Views;
using System.Linq;

namespace CodeBucket.ViewControllers.Groups
{
    public class GroupsViewController : ViewModelDrivenDialogViewController<GroupsViewModel>
	{
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Group.ToEmptyListImage(), "There are no groups."));

            ViewModel
                .Groups
                .ChangedObservable()
                .Subscribe(groups =>
                {
                    var elements = groups.Select(x =>
                    {
                        var e = new StringElement(x.Name);
                        e.BindClick(x.GoToCommand);
                        return e;
                    });

                    Root.Reset(new Section { elements });
                });
        }
	}
}

