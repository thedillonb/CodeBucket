using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Reactive.Linq;
using CodeBucket.Core.Messages;
using ReactiveUI;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : ReactiveObject
    {
        public IReactiveCommand<object> AddAccountCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> SelectAccountCommand { get; } = ReactiveCommand.Create();

        public IReactiveCommand<object> DismissCommand { get; } = ReactiveCommand.Create();

        public AccountsViewModel(IAccountsService accountsService) 
        {
            SelectAccountCommand.OfType<BitbucketAccount>().Subscribe(x =>
            {
                if (accountsService.ActiveAccount?.Id == x.Id)
                {
                    DismissCommand.ExecuteIfCan();
                }
                else
                {
                    accountsService.SetActiveAccount(x);
                    MessageBus.Current.SendMessage(new LogoutMessage());
                }
            });
        }
    }
}
