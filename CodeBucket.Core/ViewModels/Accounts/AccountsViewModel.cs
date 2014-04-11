using CodeFramework.Core.Data;
using CodeFramework.Core.Services;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AccountsViewModel : BaseAccountsViewModel
    {
        private readonly ILoginService _loginService;
        private readonly IApplicationService _applicationService;
		
        public AccountsViewModel(IAccountsService accountsService, ILoginService loginService, IApplicationService applicationService) 
            : base(accountsService)
        {
            _loginService = loginService;
            _applicationService = applicationService;
        }

        protected override void AddAccount()
        {
			this.ShowViewModel<AddAccountViewModel>();
        }

		protected async override void SelectAccount(IAccount account)
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
