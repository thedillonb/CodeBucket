using MonoTouch.Dialog;
using System.Linq;
using UIKit;
using CodeBucket.Utils;
using CodeBucket.Core.ViewModels.Issues;
using BitbucketSharp.Models;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels;
using CodeBucket.Elements;

namespace CodeBucket.Views.Issues
{
    public class IssueVersionsView : ViewModelCollectionDrivenDialogViewController
	{
        public IssueVersionsView()
		{
			Title = "Versions";
			NoItemsText = "No Versions";
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var vm = (IssueVersionsViewModel)ViewModel;
            BindCollection(vm.Versions, x => {
				var e = new VersionElement(x);
				e.Tapped += () => {
                    if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
                        vm.SelectedValue = null;
					else
                        vm.SelectedValue = x.Name;
				};
                if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
					e.Accessory = UITableViewCellAccessory.Checkmark;
				return e;
			});

            vm.Bind(x => x.SelectedValue, x =>
				{
					if (Root.Count == 0)
						return;
					foreach (var m in Root[0].Elements.Cast<VersionElement>())
						m.Accessory = (x != null && string.Equals(m.Version.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
					Root.Reload(Root[0], UITableViewRowAnimation.None);
				});

			var _hud = new Hud(View);
			vm.Bind(x => x.IsSaving, x =>
			{
				if (x) _hud.Show("Saving...");
				else _hud.Hide();
			});
		}

		private class VersionElement : StyledStringElement
		{
			public VersionModel Version { get; private set; }
			public VersionElement(VersionModel m) 
				: base(m.Name)
			{
				Version = m;
			}
		}
	}
}

