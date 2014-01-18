using System;
using CodeFramework.iOS.ViewControllers;
using CodeBucket.Core.ViewModels.Issues;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.iOS.Utils;

namespace CodeBucket.iOS
{
	public abstract class IssueModifyView : ViewModelDrivenDialogViewController
    {
		private IHud _hud;

		public new IssueModifyViewModel ViewModel
		{
			get { return (IssueModifyViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_hud = this.CreateHud();

			NavigationItem.RightBarButtonItem = new UIBarButtonItem(Theme.CurrentTheme.SaveButton, UIBarButtonItemStyle.Plain, (s, e) => {
				View.EndEditing(true);
				ViewModel.SaveCommand.Execute(null);
			});

			var title = new InputElement("Title", string.Empty, string.Empty);
			title.Changed += (object sender, EventArgs e) => ViewModel.Title = title.Value;

			var assignedTo = new StyledStringElement("Responsible", "Unassigned", UITableViewCellStyle.Value1);
			assignedTo.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			assignedTo.Tapped += () => ViewModel.GoToAssigneeCommand.Execute(null);

			var milestone = new StyledStringElement("Milestone".t(), "None", UITableViewCellStyle.Value1);
			milestone.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			milestone.Tapped += () => ViewModel.GoToMilestonesCommand.Execute(null);

			var content = new MultilinedElement("Description");
			content.Tapped += () =>
			{
				var composer = new Composer { Title = "Issue Description", Text = content.Value, ActionButtonText = "Save" };
				composer.NewComment(this, (text) => {
					ViewModel.Content = text;
					composer.CloseComposer();
				});
			};

			ViewModel.Bind(x => x.Title, x => title.Value = x);
			ViewModel.Bind(x => x.Content, x => content.Value = x);
			ViewModel.Bind(x => x.AssignedTo, x => {
				assignedTo.Value = x == null ? "Unassigned" : x.Username;
				Root.Reload(assignedTo, UITableViewRowAnimation.None);
			});
			ViewModel.Bind(x => x.Milestone, x => {
				milestone.Value = x == null ? "None" : x.Name;
				Root.Reload(milestone, UITableViewRowAnimation.None);
			});

			ViewModel.Bind(x => x.IsSaving, x =>
			{
				if (x)
					_hud.Show("Saving...");
				else
					_hud.Hide();
			});

			Root = new RootElement(Title) { new Section { title, assignedTo, milestone }, new Section { content } };
		}
    }
}

