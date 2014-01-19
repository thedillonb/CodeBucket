using System;
using CodeFramework.iOS.ViewControllers;
using CodeBucket.Core.ViewModels.Issues;
using MonoTouch.UIKit;
using MonoTouch.Dialog;
using CodeFramework.iOS.Utils;

namespace CodeBucket.iOS.Views.Issues
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

			var title = new InputElement("Title", string.Empty, string.Empty) { TextAlignment = UITextAlignment.Right };
			title.Changed += (object sender, EventArgs e) => ViewModel.Title = title.Value;

			var assignedTo = new StyledStringElement("Responsible", "Unassigned", UITableViewCellStyle.Value1);
			assignedTo.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			assignedTo.Tapped += () => ViewModel.GoToAssigneeCommand.Execute(null);

			var kind = new StyledStringElement("Issue Type", ViewModel.Kind, UITableViewCellStyle.Value1);
			kind.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			kind.Tapped += () =>
			{
				var ctrl = new IssueAttributesView(IssueModifyViewModel.Kinds, ViewModel.Kind) { Title = "Issue Type" };
				ctrl.SelectedValue = x => ViewModel.Kind = x.ToLower();
				NavigationController.PushViewController(ctrl, true);
			};

			var priority = new StyledStringElement("Priority", ViewModel.Priority, UITableViewCellStyle.Value1);
			priority.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			priority.Tapped += () => 
			{
				var ctrl = new IssueAttributesView(IssueModifyViewModel.Priorities, ViewModel.Priority) { Title = "Priority" };
				ctrl.SelectedValue = x => ViewModel.Priority = x.ToLower();
				NavigationController.PushViewController(ctrl, true);
			};

			var milestone = new StyledStringElement("Milestone".t(), "None", UITableViewCellStyle.Value1);
			milestone.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			milestone.Tapped += () => ViewModel.GoToMilestonesCommand.Execute(null);

			var component = new StyledStringElement("Component".t(), "None", UITableViewCellStyle.Value1);
			component.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            component.Tapped += () => ViewModel.GoToComponentsCommand.Execute(null);

			var version = new StyledStringElement("Version".t(), "None", UITableViewCellStyle.Value1);
			version.Accessory = UITableViewCellAccessory.DisclosureIndicator;
            version.Tapped += () => ViewModel.GoToVersionsCommand.Execute(null);

			var content = new MultilinedElement("Description");
			content.Tapped += () =>
			{
				var composer = new Composer { Title = "Issue Description", Text = content.Value, ActionButtonText = "Save" };
				composer.NewComment(this, (text) => {
					ViewModel.Content = text;
					composer.CloseComposer();
				});
			};

			ViewModel.Bind(x => x.IsSaving, x =>
			{
				if (x)
					_hud.Show("Saving...");
				else
					_hud.Hide();
			});

			Root = new RootElement(Title) { new Section { title, assignedTo, kind, priority }, new Section { milestone, component, version }, new Section { content } };

			ViewModel.Bind(x => x.Title, x => {
				title.Value = x;
				Root.Reload(title, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.AssignedTo, x => {
				assignedTo.Value = x == null ? "Unassigned" : x.Username;
				Root.Reload(assignedTo, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Kind, x => {
				kind.Value = x;
				Root.Reload(kind, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Priority, x => {
				priority.Value = x;
				Root.Reload(priority, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Milestone, x => {
				milestone.Value = x ?? "None";
				Root.Reload(milestone, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Component, x => {
				component.Value = x ?? "None";
				Root.Reload(component, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Version, x => {
				version.Value = x ?? "None";
				Root.Reload(version, UITableViewRowAnimation.None);
			}, true);

			ViewModel.Bind(x => x.Content, x => {
				content.Value = x;
				Root.Reload(content, UITableViewRowAnimation.None);
			}, true);
		}
    }
}

