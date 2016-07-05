using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueComponentsViewController : DialogViewController
	{
        private IssueComponentsViewModel ViewModel { get; }

        public IssueComponentsViewController(IssueComponentsViewModel viewModel)
            : base(UITableViewStyle.Plain)
		{
            ViewModel = viewModel;
            Title = "Components";
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            OnActivation(disposable =>
            {
                ViewModel
                    .Components.Changed
                    .Select(_ => ViewModel.Components.Select(CreateElement))
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

        private static CheckElement CreateElement(IssueAttributeItemViewModel component)
        {
            var element = new CheckElement(component.Name);
            component.WhenAnyValue(x => x.IsSelected).Subscribe(x => element.Checked = x);
            element.CheckedChanged.InvokeCommand(component.SelectCommand);
            return element;
        }
	}
}

