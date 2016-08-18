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
            ViewModel = new AccountsViewModel();
            Appearing.InvokeCommand(ViewModel.LoadCommand);

            _backButton = backButton;
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
                    .InvokeCommand(ViewModel.AddAccountCommand)
                    .AddTo(disposable);

                cancel.GetClickedObservable()
                    .InvokeCommand(ViewModel.DismissCommand)
                    .AddTo(disposable);

                ViewModel.AddAccountCommand
                    .Subscribe(_ => NavigationController.PushViewController(new NewAccountViewController(), true))
                    .AddTo(disposable);
                
                ViewModel.DismissCommand
                    .Subscribe(_ => DismissViewController(true, null))
                    .AddTo(disposable);

                ViewModel.DismissCommand.CanExecuteObservable
                         .Subscribe(x => cancel.Enabled = x)
                         .AddTo(disposable);
            });
        }
    }
}

