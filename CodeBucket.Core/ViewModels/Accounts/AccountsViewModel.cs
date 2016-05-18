using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using CodeBucket.Core.Messages;
using ReactiveUI;
using Splat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : BaseViewModel
    {
        public IReactiveCommand<object> AddAccountCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<List<BitbucketAccount>> LoadCommand { get; }

        public IReadOnlyReactiveList<AccountItemViewModel> Items { get; }

        public AccountsViewModel(IAccountsService accountsService = null) 
        {
            accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();

            Title = "Accounts";

            var accounts = new ReactiveList<BitbucketAccount>();
            Items = accounts.CreateDerivedCollection(x =>
            {
                var vm = new AccountItemViewModel(x);

                if (accountsService.ActiveAccount?.Id == x.Id)
                    vm.GoToCommand.InvokeCommand(DismissCommand);
                else
                {
                    vm.GoToCommand
                      .Select(_ => x)
                      .Do(accountsService.SetActiveAccount)
                      .Subscribe(_ => MessageBus.Current.SendMessage(new LogoutMessage()));
                }

                vm.DeleteCommand.Subscribe(_ =>
                {
                    accountsService.Remove(x);
                    accounts.Remove(x);
                });

                return vm;
            });

            LoadCommand = ReactiveCommand.CreateAsyncTask(_ => 
                Task.FromResult(new List<BitbucketAccount>(accountsService)));
            LoadCommand.Subscribe(x => accounts.Reset(x));
        }
    }
}
