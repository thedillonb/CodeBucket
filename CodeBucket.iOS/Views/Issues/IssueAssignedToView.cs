using System;
using System.Linq;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.Utils;

namespace CodeBucket.Views.Issues
{
    public class IssueAssignedToView : ViewModelCollectionDrivenDialogViewController
    {
        public IssueAssignedToView()
        {
            Title = "Assignees";
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

			var vm = (IssueAssignedToViewModel)ViewModel;
			BindCollection(vm.Users, x =>
			{
                var avatar = new Avatar(x.Avatar);
                var el = new UserElement(x.Username, string.Empty, string.Empty, avatar);
                el.Clicked.Subscribe(_ => {
					if (vm.SelectedUser != null && string.Equals(vm.SelectedUser.Username, x.Username))
						vm.SelectedUser = null;
					else
						vm.SelectedUser = x;
                });
				if (vm.SelectedUser != null && string.Equals(vm.SelectedUser.Username, x.Username, StringComparison.OrdinalIgnoreCase))
					el.Accessory = UITableViewCellAccessory.Checkmark;
				else
					el.Accessory = UITableViewCellAccessory.None;
				return el;
			});

            vm.Bind(x => x.SelectedUser).Subscribe(x =>
			{
                var elements = Root.FirstOrDefault()?.Elements ?? Enumerable.Empty<Element>();
                foreach (var m in elements.Cast<UserElement>())
					m.Accessory = (x != null && string.Equals(vm.SelectedUser.Username, x.Username, StringComparison.OrdinalIgnoreCase)) ? 
					          UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			});
        }
    }
}

