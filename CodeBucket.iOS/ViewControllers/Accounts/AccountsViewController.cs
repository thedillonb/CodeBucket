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
        public AccountsViewController()
        {
            ViewModel = new AccountsViewModel();
            Appearing.InvokeCommand(ViewModel.LoadCommand);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.Source = new AccountTableViewSource(TableView, ViewModel.Items);

            var add = NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Add);
            var cancel = NavigationItem.LeftBarButtonItem = new UIBarButtonItem { Image = Images.Buttons.Cancel };

            OnActivation(disposable =>
            {
                add.GetClickedObservable()
                    .InvokeCommand(ViewModel.AddAccountCommand)
                    .AddTo(disposable);

                cancel.GetClickedObservable()
                    .InvokeCommand(ViewModel.DismissCommand)
                    .AddTo(disposable);

                ViewModel.AddAccountCommand
                    .Subscribe(_ => NavigationController.PushViewController(new LoginViewController(), true))
                    .AddTo(disposable);
                
                ViewModel.DismissCommand
                    .Subscribe(_ => DismissViewController(true, null))
                    .AddTo(disposable);
            });
        }
    }
}

