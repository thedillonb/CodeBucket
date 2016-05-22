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

        public IssueComponentsViewController(string username, string repository)
            : base(UITableViewStyle.Plain)
		{
            ViewModel = new IssueComponentsViewModel(username, repository);
            Title = "Components";
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
            var element = new ButtonElement(component.Name);
            component.WhenAnyValue(x => x.IsSelected)
                     .Subscribe(x => element.Accessory = x ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None);
            element.BindClick(component.SelectCommand);
            return element;
        }
	}
}

