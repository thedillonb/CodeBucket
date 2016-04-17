using CodeBucket.Core.ViewModels.Groups;
using CodeBucket.ViewControllers;
using System;
using UIKit;
using CodeBucket.DialogElements;
using System.Reactive.Linq;

namespace CodeBucket.Views.Groups
{
    public class GroupsView : ViewModelCollectionDrivenDialogViewController
	{
        public GroupsView()
        {
            Title = "Groups";
            EmptyView = new Lazy<UIView>(() =>
                new EmptyListView(AtlassianIcon.Group.ToEmptyListImage(), "There are no groups."));
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var vm = (GroupsViewModel) ViewModel;
            var weakVm = new WeakReference<GroupsViewModel>(vm);
			BindCollection(vm.Groups, x => {
                var e = new StringElement(x.Name);
                e.Clicked.Select(_ => x).BindCommand(weakVm.Get()?.GoToGroupCommand);
				return e;
			});
        }
	}
}

