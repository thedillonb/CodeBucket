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
			BindCollection(vm.Milestones, x => {
				var e = new MilestoneElement(x);
                e.Clicked.Subscribe(_ => {
					if (vm.SelectedMilestone != null && string.Equals(vm.SelectedMilestone, x.Name))
						vm.SelectedMilestone = null;
					else
						vm.SelectedMilestone = x.Name;
                });
				if (vm.SelectedMilestone != null && string.Equals(vm.SelectedMilestone, x.Name))
					e.Accessory = UITableViewCellAccessory.Checkmark;
				return e;
			});

            vm.Bind(x => x.SelectedMilestone).Subscribe(x =>
			{
                var elements = Root.FirstOrDefault()?.Elements ?? Enumerable.Empty<Element>();
                foreach (var m in elements.Cast<MilestoneElement>())
					m.Accessory = (x != null && string.Equals(m.Milestone.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
			});

            OnActivation(d => d(vm.Bind(x => x.IsSaving).SubscribeStatus("Saving...")));
		}

        private class MilestoneElement : StringElement
		{
			public MilestoneModel Milestone { get; private set; }
			public MilestoneElement(MilestoneModel m) 
				: base(m.Name)
			{
				Milestone = m;
			}
		}
	}
}

