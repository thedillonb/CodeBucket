using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using BitbucketSharp.Models;
using ReactiveUI;

namespace CodeBucket.Views.Issues
{
    public class IssueVersionsView : ViewModelDrivenDialogViewController<IssueVersionsViewModel>
	{
        public IssueVersionsView()
		{
			Title = "Versions";
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

//            BindCollection(vm.Versions, x => {
//				var e = new VersionElement(x);
//                e.Clicked.Subscribe(_ => {
//                    if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
//                        vm.SelectedValue = null;
//					else
//                        vm.SelectedValue = x.Name;
//                });
//                if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
//					e.Accessory = UITableViewCellAccessory.Checkmark;
//				return e;
//			});
//
            ViewModel.WhenAnyValue(x => x.SelectedValue).Subscribe(x =>
				{
					if (Root.Count == 0) return;
					foreach (var m in Root[0].Elements.Cast<VersionElement>())
						m.Accessory = (x != null && string.Equals(m.Version.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
				});
		}

        private class VersionElement : StringElement
		{
            public IssueVersion Version { get; private set; }
            public VersionElement(IssueVersion m) 
				: base(m.Name)
			{
				Version = m;
			}
		}
	}
}

