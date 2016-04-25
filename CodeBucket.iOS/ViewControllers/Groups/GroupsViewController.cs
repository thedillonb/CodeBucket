using CodeBucket.Core.ViewModels.Groups;
using System;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Views;
using System.Linq;

namespace CodeBucket.ViewControllers.Groups
{
    public class GroupsViewController : ViewModelCollectionDrivenDialogViewController
	{
        public GroupsViewController()
        {
            Title = "Groups";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Group.ToEmptyListImage(), "There are no groups."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var vm = (GroupsViewModel) ViewModel;
            vm.Groups.ChangedObservable().Subscribe(groups =>
            {
                var elements = groups.Select(x =>
                {
                    var e = new StringElement(x.Name);
                    e.Clicked.BindCommand(x.GoToCommand);
                    return e;
                });

                Root.Reset(new Section { elements });
            });
        }
	}
}

