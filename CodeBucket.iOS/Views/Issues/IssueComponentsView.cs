using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.Client.Models;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;

namespace CodeBucket.Views.Issues
{
    public class IssueComponentsView : ViewModelCollectionDrivenDialogViewController
	{
        public IssueComponentsView()
		{
            Title = "Components";
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var vm = (IssueComponentsViewModel)ViewModel;
            BindCollection(vm.Components, x => {
                var e = new ComponentElement(x);
                e.Clicked.Subscribe(_ => {
                    if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
                        vm.SelectedValue = null;
					else
                        vm.SelectedValue = x.Name;
                });
                if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
					e.Accessory = UITableViewCellAccessory.Checkmark;
				return e;
			});

            vm.Bind(x => x.SelectedValue).Subscribe(x =>
				{
					if (Root.Count == 0) return;
                    foreach (var m in Root[0].Elements.Cast<ComponentElement>())
                        m.Accessory = (x != null && string.Equals(m.Component.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				});

            OnActivation(d => d(vm.Bind(x => x.IsSaving).SubscribeStatus("Saving...")));
		}

        private class ComponentElement : StringElement
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

