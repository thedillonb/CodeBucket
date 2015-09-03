using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Utils;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : BaseViewModel
    {
        private readonly IAccountsService _accountsService;
        private readonly CustomObservableCollection<IAccount> _accounts = new CustomObservableCollection<IAccount>();
        private bool _isLoggingIn;
        private readonly ILoginService _loginService;
        private readonly IApplicationService _applicationService;


        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set
            {
                _isLoggingIn = value;
                RaisePropertyChanged(() => IsLoggingIn);
            }
        }

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
            get { return new MvxCommand<IAccount>(SelectAccount); }
        }

        public void Init()
        {
            _accounts.Reset(_accountsService);
        }

        public AccountsViewModel(IAccountsService accountsService, ILoginService loginService, IApplicationService applicationService) 
        {
            _accountsService = accountsService;
            _loginService = loginService;
            _applicationService = applicationService;
        }

        protected void AddAccount()
        {
			this.ShowViewModel<AddAccountViewModel>();
        }

		protected async void SelectAccount(IAccount account)
        {
			var githubAccount = (BitbucketAccount) account;

			if (githubAccount.DontRemember)
			{
				ShowViewModel<AddAccountViewModel>(new AddAccountViewModel.NavObject { AttemptedAccountId = githubAccount.Id });
				return;
			}

			try
			{
				IsLoggingIn = true;
				var client = await _loginService.LoginAccount(githubAccount);
				_applicationService.ActivateUser(githubAccount, client);
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to login: " + e.Message);
				ShowViewModel<AddAccountViewModel>(new AddAccountViewModel.NavObject() { AttemptedAccountId = githubAccount.Id });
			}
			finally
			{
				IsLoggingIn = false;
			}
        }
    }
}
