using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.ViewControllers;
using CodeBucket.DialogElements;
using BitbucketSharp.Models;

namespace CodeBucket.Views.Issues
{
	public class IssueMilestonesView : ViewModelCollectionDrivenDialogViewController
	{
		public IssueMilestonesView()
		{
			Title = "Milestones";
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var vm = (IssueMilestonesViewModel)ViewModel;
//            vm.Bind(x => x.Milestones, true).Subscribe(milestones =>
//            {
//                var items = milestones ?? Enumerable.Empty<IssueMilestone>();
//                var elements = items.Select(x => 
//                {
//                    var e = new MilestoneElement(x);
//                    e.Clicked.Subscribe(_ => {
//                        if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
//                            vm.SelectedValue = null;
//                        else
//                            vm.SelectedValue = x.Name;
//                    });
//                    if (vm.SelectedValue != null && string.Equals(vm.SelectedValue, x.Name))
//                        e.Accessory = UITableViewCellAccessory.Checkmark;
//
//                    return e;
//                });
//
//                Root.Reset(new Section() { elements });
//            });
//
            vm.Bind(x => x.SelectedValue).Subscribe(x =>
			{
                var elements = Root.FirstOrDefault()?.Elements ?? Enumerable.Empty<Element>();
                foreach (var m in elements.Cast<MilestoneElement>())
					m.Accessory = (x != null && string.Equals(m.Milestone.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			});
		}

        private class MilestoneElement : StringElement
		{
			public IssueMilestone Milestone { get; private set; }
            public MilestoneElement(IssueMilestone m) 
				: base(m.Name)
			{
				Milestone = m;
			}
		}
	}
}

