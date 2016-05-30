using System;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;
using ReactiveUI;
using CodeBucket.TableViewSources;
using System.Reactive.Linq;
using Humanizer;

namespace CodeBucket.Views.Issues
{
    public abstract class IssueModifyViewController<TViewModel> : TableViewController<TViewModel>
        where TViewModel : IssueModifyViewModel
    {
        private readonly Lazy<RootElement> _root;

        protected IssueModifyViewController()
        {
            _root = new Lazy<RootElement>(() => new RootElement(TableView));
        }

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var root = _root.Value;
            TableView.Source = new DialogTableViewSource(root);
            TableView.TableFooterView = new UIView();

            var save = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save);

            var title = new EntryElement("Title", string.Empty, string.Empty) { TextAlignment = UITextAlignment.Right };
            var assignedTo = new ButtonElement("Responsible", "Unassigned", UITableViewCellStyle.Value1);
            var kind = new ButtonElement("Issue Type", ViewModel.Kind, UITableViewCellStyle.Value1);
            var priority = new ButtonElement("Priority", ViewModel.Priority, UITableViewCellStyle.Value1);
            var milestone = new ButtonElement("Milestone", "None", UITableViewCellStyle.Value1);
            var component = new ButtonElement("Component", "None", UITableViewCellStyle.Value1);
            var version = new ButtonElement("Version", "None", UITableViewCellStyle.Value1);
            var content = new ExpandingInputElement("Description");

            var titleSection = new Section { title, assignedTo, kind, priority };
            var attributeSection = new Section("Categories") { milestone, component, version };
            var contentSection = new Section("Content") { content };
            root.Reset(titleSection, attributeSection, contentSection);

            OnActivation(d =>
            {
                this.WhenAnyValue(x => x.ViewModel.IsSaving).SubscribeStatus("Saving...").AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Title)
                    .Subscribe(x => title.Value = x)
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.AssignedTo)
                    .Subscribe(x => assignedTo.Value = x == null ? "Unassigned" : x.Username)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Kind)
                    .Select(x => x?.Humanize(LetterCasing.Title))
                    .Subscribe(x => kind.Value = x)
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Priority)
                    .Select(x => x?.Humanize(LetterCasing.Title))
                    .Subscribe(x => priority.Value = x)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Milestone)
                    .Subscribe(x => milestone.Value = x ?? "None")
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Component)
                    .Subscribe(x => component.Value = x ?? "None")
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Version)
                    .Subscribe(x => version.Value = x ?? "None")
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Content)
                    .Subscribe(x => version.Value = x)
                    .AddTo(d);

                title.Changed.Subscribe(x =>  ViewModel.IssueTitle = x).AddTo(d);
                version.Clicked.BindCommand(ViewModel.GoToVersionsCommand).AddTo(d);
                assignedTo.Clicked.BindCommand(ViewModel.GoToAssigneeCommand).AddTo(d);
                milestone.Clicked.BindCommand(ViewModel.GoToMilestonesCommand).AddTo(d);
                component.Clicked.BindCommand(ViewModel.GoToComponentsCommand).AddTo(d);

                save.GetClickedObservable().Subscribe(_ => {
                    View.EndEditing(true);
                    ViewModel.SaveCommand.Execute(null);
                }).AddTo(d);

                priority.Clicked.Subscribe(_ => 
                {
                    var ctrl = new IssueAttributesViewController(
                        IssueAttributesViewController.Priorities, ViewModel.Priority) { Title = "Priority" };
                    ctrl.SelectedObservable
                        .Do(x => ViewModel.Priority = x.ToLower())
                        .Subscribe(__ => NavigationController.PopToViewController(this, true));
                    NavigationController.PushViewController(ctrl, true);
                }).AddTo(d);

                kind.Clicked.Subscribe(_ => 
                {
                    var ctrl = new IssueAttributesViewController(
                        IssueAttributesViewController.Kinds, ViewModel.Kind) { Title = "Issue Type" };
                    ctrl.SelectedObservable
                        .Do(x => ViewModel.Kind = x.ToLower())
                        .Subscribe(__ => NavigationController.PopToViewController(this, true));
                    NavigationController.PushViewController(ctrl, true);
                }).AddTo(d);
            });
		}
    }
}

