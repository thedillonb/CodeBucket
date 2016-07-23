using System;
using UIKit;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Issues;
using ReactiveUI;
using System.Reactive.Linq;
using System.Linq;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueMilestonesViewController : DialogViewController
    {
       public IssueMilestonesViewModel ViewModel { get; }

       public IssueMilestonesViewController(string username, string repository)
            : this(new IssueMilestonesViewModel(username, repository))
        {
        }

        public IssueMilestonesViewController(IssueMilestonesViewModel viewModel)
            : base(UITableViewStyle.Plain)
        {
            ViewModel = viewModel;
            Title = "Milestones";
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            OnActivation(disposable =>
            {
                ViewModel
                    .Milestones.Changed
                    .Select(_ => ViewModel.Milestones.Select(CreateElement))
                    .Subscribe(x => Root.Reset(new Section { x }))
                    .AddTo(disposable);

                ViewModel
                    .LoadCommand
                    .IsExecuting
                    .Subscribe(x => TableView.IsLoading = x)
                    .AddTo(disposable);
                
                ViewModel
                    .DismissCommand
                    .Subscribe(_ => NavigationController.PopViewController(true))
                    .AddTo(disposable);
            });

            ViewModel.LoadCommand.ExecuteIfCan();
        }

        private static CheckElement CreateElement(IssueAttributeItemViewModel attribute)
        {
            var element = new CheckElement(attribute.Name);
            attribute.WhenAnyValue(x => x.IsSelected).Subscribe(x => element.Checked = x);
            element.CheckedChanged.InvokeCommand(attribute.SelectCommand);
            return element;
        }
    }
}

