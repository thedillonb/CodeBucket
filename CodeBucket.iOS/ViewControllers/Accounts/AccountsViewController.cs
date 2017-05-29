using System;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Accounts;
using CodeBucket.TableViewSources;
using ReactiveUI;
using UIKit;

namespace CodeBucket.ViewControllers.Accounts
{
    public class AccountsViewController : TableViewController<AccountsViewModel>
    {
        private readonly bool _backButton;

        public AccountsViewController(bool backButton)
        {
            _backButton = backButton;
            ViewModel = new AccountsViewModel();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new AccountTableViewSource(TableView, ViewModel.Items);

            var add = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var cancel = new UIBarButtonItem { Image = Images.Buttons.Cancel };

            if (_backButton)
                NavigationItem.LeftBarButtonItem = cancel;

            OnActivation(disposable =>
            {
                add.GetClickedObservable()
                    .SelectUnit()
                    .BindCommand(ViewModel.AddAccountCommand)
                    .AddTo(disposable);

                cancel.GetClickedObservable()
                    .SelectUnit()
                    .BindCommand(ViewModel.DismissCommand)
                    .AddTo(disposable);

                ViewModel.AddAccountCommand
                    .Subscribe(_ => NavigationController.PushViewController(new LoginViewController(), true))
                    .AddTo(disposable);
                
                ViewModel.DismissCommand
                    .Subscribe(_ => DismissViewController(true, null))
                    .AddTo(disposable);

                ViewModel.DismissCommand.CanExecute
                    .Subscribe(x => cancel.Enabled = x)
                    .AddTo(disposable);
            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            ViewModel.LoadCommand.ExecuteNow();
        }
    }
}

