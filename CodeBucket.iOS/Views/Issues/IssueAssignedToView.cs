using System;
using System.Linq;
using CodeBucket.Elements;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels;
using Cirrious.MvvmCross.ViewModels;

namespace CodeBucket.Views.Issues
{
    public class IssueAssignedToView : ViewModelCollectionDrivenDialogViewController
    {
        public override void ViewDidLoad()
        {
            Title = "Assignees";
            NoItemsText = "No Assignees";

            base.ViewDidLoad();

			var vm = (IssueAssignedToViewModel)ViewModel;
			BindCollection(vm.Users, x =>
			{
				var el = new UserElement(x.Username, string.Empty, string.Empty, x.Avatar);
				el.Tapped += () => {
					if (vm.SelectedUser != null && string.Equals(vm.SelectedUser.Username, x.Username))
						vm.SelectedUser = null;
					else
						vm.SelectedUser = x;
				};
				if (vm.SelectedUser != null && string.Equals(vm.SelectedUser.Username, x.Username, StringComparison.OrdinalIgnoreCase))
					el.Accessory = UITableViewCellAccessory.Checkmark;
				else
					el.Accessory = UITableViewCellAccessory.None;
				return el;
			});

			vm.Bind(x => x.SelectedUser, x =>
			{
				if (Root.Count == 0)
					return;
				foreach (var m in Root[0].Elements.Cast<UserElement>())
					m.Accessory = (x != null && string.Equals(vm.SelectedUser.Username, x.Username, StringComparison.OrdinalIgnoreCase)) ? 
					          UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				Root.Reload(Root[0], UITableViewRowAnimation.None);
			});
        }
    }
}

