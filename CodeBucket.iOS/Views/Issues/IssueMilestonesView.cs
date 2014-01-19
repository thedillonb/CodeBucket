using CodeFramework.ViewControllers;
using MonoTouch.Dialog;
using System.Linq;
using MonoTouch.UIKit;
using CodeFramework.iOS.Utils;
using CodeBucket.Core.ViewModels.Issues;
using BitbucketSharp.Models;

namespace CodeBucket.iOS.Views.Issues
{
	public class IssueMilestonesView : ViewModelCollectionDrivenDialogViewController
	{
		public IssueMilestonesView()
		{
			Title = "Milestones".t();
			NoItemsText = "No Milestones".t();
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var vm = (IssueMilestonesViewModel)ViewModel;
			BindCollection(vm.Milestones, x => {
				var e = new MilestoneElement(x);
				e.Tapped += () => {
					if (vm.SelectedMilestone != null && string.Equals(vm.SelectedMilestone, x.Name))
						vm.SelectedMilestone = null;
					else
						vm.SelectedMilestone = x.Name;
				};
				if (vm.SelectedMilestone != null && string.Equals(vm.SelectedMilestone, x.Name))
					e.Accessory = UITableViewCellAccessory.Checkmark;
				return e;
			});

			vm.Bind(x => x.SelectedMilestone, x =>
				{
					if (Root.Count == 0)
						return;
					foreach (var m in Root[0].Elements.Cast<MilestoneElement>())
						m.Accessory = (x != null && string.Equals(m.Milestone.Name, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
					Root.Reload(Root[0], UITableViewRowAnimation.None);
				});

			var _hud = new Hud(View);
			vm.Bind(x => x.IsSaving, x =>
			{
				if (x) _hud.Show("Saving...");
				else _hud.Hide();
			});
		}

		private class MilestoneElement : StyledStringElement
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

