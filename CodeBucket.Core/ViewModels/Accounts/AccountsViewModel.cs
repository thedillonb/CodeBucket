using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : BaseViewModel
    {
        private readonly IAccountsService _accountsService;
        private readonly CustomObservableCollection<IAccount> _accounts = new CustomObservableCollection<IAccount>();

        public CustomObservableCollection<IAccount> Accounts
        {
            get { return _accounts; }
        }

        public ICommand AddAccountCommand
        {
            get { return new MvxCommand(AddAccount); }
        }

        public ICommand SelectAccountCommand
        {
            get { return new MvxCommand<BitbucketAccount>(SelectAccount); }
        }

        public void Init()
        {
            _accounts.Reset(_accountsService);
        }

        public AccountsViewModel(IAccountsService accountsService) 
        {
            _accountsService = accountsService;
        }

        private void AddAccount()
        {
            this.ShowViewModel<LoginViewModel>();
        }

        private void SelectAccount(BitbucketAccount account)
        {
            _accountsService.SetActiveAccount(account);
            ShowViewModel<CodeBucket.Core.ViewModels.App.StartupViewModel>();
        }
    }
}
