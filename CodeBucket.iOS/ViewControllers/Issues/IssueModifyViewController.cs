using System;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Utilities;
using ReactiveUI;
using CodeBucket.TableViewSources;
using System.Reactive.Linq;
using Humanizer;

namespace CodeBucket.ViewControllers.Issues
{
    public abstract class IssueModifyViewController<TViewModel> : TableViewController<TViewModel>
        where TViewModel : IssueModifyViewModel
    {
        private readonly Lazy<IssueComponentsViewController> _componentsViewController;
        private readonly Lazy<IssueVersionsViewController> _versionsViewController;
        private readonly Lazy<IssueMilestonesViewController> _milestonesViewController;
        private readonly Lazy<IssueAssigneeViewController> _assigneeViewController;
        private readonly Lazy<RootElement> _root;

        protected RootElement Root => _root.Value;

        protected IssueModifyViewController()
        {
            _root = new Lazy<RootElement>(() => new RootElement(TableView));

            _componentsViewController = new Lazy<IssueComponentsViewController>(
                () => new IssueComponentsViewController(ViewModel.Components));

            _versionsViewController = new Lazy<IssueVersionsViewController>(
                () => new IssueVersionsViewController(ViewModel.Versions));

            _milestonesViewController = new Lazy<IssueMilestonesViewController>(
                () => new IssueMilestonesViewController(ViewModel.Milestones));

            _assigneeViewController = new Lazy<IssueAssigneeViewController>(
                () => new IssueAssigneeViewController(ViewModel.Assignee));
        }

        public void Present(UIViewController presenter)
        {
            NavigationItem.LeftBarButtonItem = new UIBarButtonItem { Image = Images.Buttons.Cancel };
            NavigationItem.LeftBarButtonItem.GetClickedObservable().InvokeCommand(ViewModel.DiscardCommand);
            presenter.PresentViewController(new ThemedNavigationController(this), true, null);
        }

        public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            var root = _root.Value;
            TableView.Source = new DialogTableViewSource(root);
            TableView.TableFooterView = new UIView();

            var save = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Save);

            var title = new EntryElement("Title", "(Required)", string.Empty);
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
                this.WhenAnyObservable(x => x.ViewModel.SaveCommand.IsExecuting)
                    .SubscribeStatus("Saving...")
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.IssueTitle)
                    .Subscribe(x => title.Value = x)
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Assignee.SelectedValue)
                    .Subscribe(x => assignedTo.Value = x ?? "Unassigned")
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Kind)
                    .Select(x => x?.Humanize(LetterCasing.Title))
                    .Subscribe(x => kind.Value = x)
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Priority)
                    .Select(x => x?.Humanize(LetterCasing.Title))
                    .Subscribe(x => priority.Value = x)
                    .AddTo(d);

                this.WhenAnyValue(x => x.ViewModel.Milestones.SelectedValue)
                    .Subscribe(x => milestone.Value = x ?? "None")
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Components.SelectedValue)
                    .Subscribe(x => component.Value = x ?? "None")
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Versions.SelectedValue)
                    .Subscribe(x => version.Value = x ?? "None")
                    .AddTo(d);

                content
                    .Changed
                    .Subscribe(x => ViewModel.Content = x)
                    .AddTo(d);
                
                this.WhenAnyValue(x => x.ViewModel.Content)
                    .Subscribe(x => content.Value = x)
                    .AddTo(d);

                title.Changed
                    .Subscribe(x =>  ViewModel.IssueTitle = x)
                    .AddTo(d);

                version
                    .Clicked
                    .Select(_ => _versionsViewController.Value)
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(d);
                
                assignedTo
                    .Clicked
                    .Select(_ => _assigneeViewController.Value)
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(d);

                milestone
                    .Clicked
                    .Select(_ => _milestonesViewController.Value)
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(d);
                
                component
                    .Clicked
                    .Select(_ => _componentsViewController.Value)
                    .Subscribe(x => NavigationController.PushViewController(x, true))
                    .AddTo(d);

                save.GetClickedObservable()
                    .Do(_ => View.EndEditing(true))
                    .InvokeCommand(this, x => x.ViewModel.SaveCommand)
                    .AddTo(d);

                this.WhenAnyObservable(x => x.ViewModel.SaveCommand.CanExecuteObservable)
                    .Subscribe(x => save.Enabled = x)
                    .AddTo(d);

                this.WhenAnyObservable(x => x.ViewModel.DismissCommand)
                    .Subscribe(x => DismissViewController(true, null))
                    .AddTo(d);

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

