using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using CodeBucket.Core.Messages;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Linq;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : BaseViewModel
    {
        public IReactiveCommand<object> AddAccountCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> DismissCommand { get; }

        public IReactiveCommand<List<Account>> LoadCommand { get; }

        public IReadOnlyReactiveList<AccountItemViewModel> Items { get; }

        public AccountsViewModel(
            IApplicationService applicationService = null,
            IAccountsService accountsService = null) 
        {
            accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Accounts";

            var activeAccount = applicationService.Account;
            var currentUsername = activeAccount?.Username;

            var accounts = new ReactiveList<Account>();
            Items = accounts.CreateDerivedCollection(x =>
            {
                var vm = new AccountItemViewModel(x);

                if (activeAccount?.Id == x.Id)
                {
                    vm.GoToCommand.InvokeCommand(DismissCommand);
                    vm.IsSelected = true;
                }
                else
                {
                    vm.GoToCommand
                      .Do(_ => applicationService.SetDefaultAccount(x))
                      .Subscribe(_ => MessageBus.Current.SendMessage(new LogoutMessage()));
                }

                vm.DeleteCommand.Subscribe(_ =>
                {
                    accountsService.Remove(x);
                    accounts.Remove(x);
                });

                return vm;
            });

            DismissCommand = ReactiveCommand.Create(
                accounts.Changed.Select(x => accounts.Any(y => y.Username == currentUsername)));

            LoadCommand = ReactiveCommand.CreateAsyncTask(async _ =>
            {
                var allAccounts = await accountsService.GetAccounts();
                return allAccounts.ToList();
            });
            LoadCommand.Subscribe(x => accounts.Reset(x));
        }
    }
}
