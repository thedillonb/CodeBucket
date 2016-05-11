using System;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;
using ReactiveUI;

namespace CodeBucket.Views.Issues
{
    public abstract class IssueModifyView<TViewModel> : ViewModelDrivenDialogViewController<TViewModel>
        where TViewModel : IssueModifyViewModel
    {
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var save = NavigationItem.RightBarButtonItem = new UIBarButtonItem { Image = Images.Buttons.Save };

            var title = new EntryElement("Title", string.Empty, string.Empty) { TextAlignment = UITextAlignment.Right };
            var assignedTo = new StringElement("Responsible", "Unassigned", UITableViewCellStyle.Value1);
            var kind = new StringElement("Issue Type", ViewModel.Kind, UITableViewCellStyle.Value1);
            var priority = new StringElement("Priority", ViewModel.Priority, UITableViewCellStyle.Value1);
            var milestone = new StringElement("Milestone", "None", UITableViewCellStyle.Value1);
            var component = new StringElement("Component", "None", UITableViewCellStyle.Value1);
            var version = new StringElement("Version", "None", UITableViewCellStyle.Value1);
			var content = new MultilinedElement("Description");
	
            Root.Reset(new Section { title, assignedTo, kind, priority }, new Section { milestone, component, version }, new Section { content });

            OnActivation(d =>
            {
                d(ViewModel.WhenAnyValue(x => x.IsSaving).SubscribeStatus("Saving..."));

                d(ViewModel.WhenAnyValue(x => x.Title).Subscribe(x => title.Value = x));
                d(ViewModel.WhenAnyValue(x => x.AssignedTo).Subscribe(x => assignedTo.Value = x == null ? "Unassigned" : x.Username));

                d(ViewModel.WhenAnyValue(x => x.Kind).Subscribe(x => kind.Value = x));
                d(ViewModel.WhenAnyValue(x => x.Priority).Subscribe(x => priority.Value = x));
                d(ViewModel.WhenAnyValue(x => x.Milestone).Subscribe(x => milestone.Value = x ?? "None"));
                d(ViewModel.WhenAnyValue(x => x.Component).Subscribe(x => component.Value = x ?? "None"));
                d(ViewModel.WhenAnyValue(x => x.Version).Subscribe(x => version.Value = x ?? "None"));
                d(ViewModel.WhenAnyValue(x => x.Content).Subscribe(x => version.Value = x));

                d(title.Changed.Subscribe(x =>  ViewModel.IssueTitle = x));
                d(version.Clicked.BindCommand(ViewModel.GoToVersionsCommand));
                d(assignedTo.Clicked.BindCommand(ViewModel.GoToAssigneeCommand));
                d(milestone.Clicked.BindCommand(ViewModel.GoToMilestonesCommand));
                d(component.Clicked.BindCommand(ViewModel.GoToComponentsCommand));

                d(save.GetClickedObservable().Subscribe(_ => {
                    View.EndEditing(true);
                    ViewModel.SaveCommand.Execute(null);
                }));

                d(content.Clicked.Subscribe(_ => 
                {
                    var composer = new Composer { Title = "Issue Description", Text = ViewModel.Content };
                    composer.NewComment(this, (text) => {
                        ViewModel.Content = text;
                        composer.CloseComposer();
                    });
                }));

                d(priority.Clicked.Subscribe(_ => 
                {
                    var ctrl = new IssueAttributesView(IssueModifyViewModel.Priorities, ViewModel.Priority) { Title = "Priority" };
                    ctrl.SelectedValue = x => ViewModel.Priority = x.ToLower();
                    NavigationController.PushViewController(ctrl, true);
                }));

                d(kind.Clicked.Subscribe(_ => 
                {
                    var ctrl = new IssueAttributesView(IssueModifyViewModel.Kinds, ViewModel.Kind) { Title = "Issue Type" };
                    ctrl.SelectedValue = x => ViewModel.Kind = x.ToLower();
                    NavigationController.PushViewController(ctrl, true);
                }));
            });
		}
    }
}

