using System;
using CodeBucket.DialogElements;
using CodeBucket.Core.ViewModels.Issues;
using UIKit;
using ReactiveUI;
using System.Linq;
using CodeBucket.Core.Utils;
using System.Reactive.Linq;

namespace CodeBucket.ViewControllers.Issues
{
    public class IssueAssigneeViewController : DialogViewController
    {
        private IssueAssigneeViewModel ViewModel { get; }

        public IssueAssigneeViewController(IssueAssigneeViewModel viewModel)
            : base(UITableViewStyle.Plain)
        {
            ViewModel = viewModel;
            Title = "Assigned To";
            EnableSearch = false;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            OnActivation(disposable =>
            {
                ViewModel
                    .Assignees.Changed
                    .Select(_ => ViewModel.Assignees.Select(CreateElement))
                    .Subscribe(x => Root.Reset(new Section { x }));

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

            ViewModel.LoadCommand.ExecuteNow();
        }

        private CheckElement CreateElement(IssueAssigneeItemViewModel item)
        {
            var element = new CheckUserElement(item.Name, item.Avatar);
            item.WhenAnyValue(x => x.IsSelected).Subscribe(x => element.Checked = x);
            element.CheckedChanged.SelectUnit().BindCommand(item.SelectCommand);
            return element;
        }

        private class CheckUserElement : CheckElement
        {
            private readonly Avatar _avatar;

            public CheckUserElement(string name, Avatar avatar)
                : base(name)
            {
                _avatar = avatar;
            }

            protected override UITableViewCell InitializeCell(UITableViewCell cell)
            {
                var c = cell as CheckUserTableViewCell;
                if (c == null)
                    return base.InitializeCell(cell);

                c.Set(Caption, _avatar);
                c.Accessory = Checked ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                return c;
            }

            protected override UITableViewCell CreateTableViewCell(UITableViewCellStyle style, string key)
            {
                return new CheckUserTableViewCell();
            }
        }

        private class CheckUserTableViewCell : UITableViewCell
        {
            public CheckUserTableViewCell()
            {
                SeparatorInset = new UIEdgeInsets(0, 48f, 0, 0);
                ImageView.ContentMode = UIViewContentMode.ScaleAspectFill;
                ImageView.Layer.CornerRadius = 16f;
                ImageView.Layer.MasksToBounds = true;
            }

            public void Set(string name, Avatar avatar)
            {
                ImageView.SetAvatar(avatar);
                TextLabel.Text = name;
            }

            public override void LayoutSubviews()
            {
                base.LayoutSubviews();
                ImageView.Frame = new CoreGraphics.CGRect(6, 6, 32, 32);
                TextLabel.Frame = new CoreGraphics.CGRect(48, TextLabel.Frame.Y, TextLabel.Frame.Width, TextLabel.Frame.Height);
                if (DetailTextLabel != null)
                    DetailTextLabel.Frame = new CoreGraphics.CGRect(48, DetailTextLabel.Frame.Y, DetailTextLabel.Frame.Width, DetailTextLabel.Frame.Height);
            }
        }
    }
}

