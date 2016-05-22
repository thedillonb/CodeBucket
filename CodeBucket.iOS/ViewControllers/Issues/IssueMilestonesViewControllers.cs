using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using ReactiveUI;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueMilestonesViewControllers : DialogViewController
	{
        private IssueComponentsViewModel ViewModel { get; } 
     
        public IssueMilestonesViewControllers(string username, string repository)
            : base(UITableViewStyle.Plain)
        {
            ViewModel = new IssueComponentsViewModel(username, repository);
            Title = "Milestones";
			EnableSearch = false;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.Components.Changed.Subscribe(_ =>
            {
                var elements = ViewModel.Components.Select(CreateElement);
                Root.Reset(new Section { elements });
            });

            ViewModel.LoadCommand.ExecuteIfCan();
        }

        private StringElement CreateElement(IssueComponentItemViewModel component)
        {
            var element = new CheckElement(component.Name);
            component.WhenAnyValue(x => x.IsSelected).Subscribe(x => element.Checked = x);
            element.Clicked.InvokeCommand(component.SelectCommand);
            return element;
        }
	}
}

