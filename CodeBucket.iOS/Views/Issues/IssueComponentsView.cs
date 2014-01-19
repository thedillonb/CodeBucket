using CodeFramework.ViewControllers;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.UIKit;
using CodeFramework.iOS.Utils;
using CodeBucket.Core.ViewModels.Issues;
using BitbucketSharp.Models;

namespace CodeBucket.iOS.Views.Issues
{
    public class IssueComponentsView : ViewModelCollectionDrivenDialogViewController
	{
        public IssueComponentsView()
		{
            Title = "Components".t();
            NoItemsText = "No Components".t();
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var vm = (IssueComponentsViewModel)ViewModel;
            BindCollection(vm.Components, x => {
                var e = new ComponentElement(x);
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
                    foreach (var m in Root[0].Elements.Cast<ComponentElement>())
                        m.Accessory = (x != null && string.Equals(m.Component.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
					Root.Reload(Root[0], UITableViewRowAnimation.None);
				});

			var _hud = new Hud(View);
			vm.Bind(x => x.IsSaving, x =>
			{
				if (x) _hud.Show("Saving...");
				else _hud.Hide();
			});
		}

        private class ComponentElement : StyledStringElement
		{
            public ComponentModel Component { get; private set; }
            public ComponentElement(ComponentModel m) 
				: base(m.Name)
			{
                Component = m;
			}
		}
	}
}

