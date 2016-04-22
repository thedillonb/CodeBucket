using System;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Linq;
using CodeBucket.Core.Utils;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueAssigneeViewController : DialogViewController
    {
        private IssueAssigneeViewModel ViewModel { get; }

        public IssueAssigneeViewController(string username, string repository)
            : base(UITableViewStyle.Plain)
        {
            Title = "Components";
            EnableSearch = false;

            ViewModel = new IssueAssigneeViewModel(MvvmCross.Platform.Mvx.Resolve<IApplicationService>());
            ViewModel.Init(username, repository);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ViewModel.Assignees.Changed.Subscribe(_ =>
            {
                var elements = ViewModel.Assignees.Select(CreateElement);
                Root.Reset(new Section { elements });
            });

            ViewModel.LoadCommand.ExecuteIfCan();
        }

        private StringElement CreateElement(IssueAssigneeItemViewModel item)
        {
            var element = new StringElement(item.Name) { ImageUri = item.Avatar.ToUri() };
            item.WhenAnyValue(x => x.IsSelected)
                     .Subscribe(x => element.Accessory = x ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None);
            element.BindClick(item.SelectCommand);
            return element;
        }
    }
}

