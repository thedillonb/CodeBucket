using System;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
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
            ViewModel = new IssueAssigneeViewModel(username, repository);
            Title = "Components";
            EnableSearch = false;
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

