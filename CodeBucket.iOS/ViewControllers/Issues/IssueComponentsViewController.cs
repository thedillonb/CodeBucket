using System;
using System.Linq;
using UIKit;
using CodeBucket.Core.ViewModels.Issues;
using CodeBucket.DialogElements;
using CodeBucket.Core.Services;
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
            Title = "Components";
			EnableSearch = false;

            ViewModel = new IssueComponentsViewModel(MvvmCross.Platform.Mvx.Resolve<IApplicationService>());
            ViewModel.Init(username, repository);
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
            var element = new StringElement(component.Name);
            component.WhenAnyValue(x => x.IsSelected)
                     .Subscribe(x => element.Accessory = x ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None);
            element.BindClick(component.SelectCommand);
            return element;
        }
	}
}

