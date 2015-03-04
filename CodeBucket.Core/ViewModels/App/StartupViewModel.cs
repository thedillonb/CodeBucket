using System;
using CodeFramework.Core.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Linq;
using System.Text.RegularExpressions;

namespace CodeBucket.Core.ViewModels.App
{
	public class StartupViewModel : BaseStartupViewModel
    {
		private readonly ILoginService _loginService;
		private readonly IApplicationService _applicationService;

		public StartupViewModel(ILoginService loginService, IApplicationService applicationService)
		{
			_loginService = loginService;
			_applicationService = applicationService;
		}

		protected async override void Startup()
		{
			if (!_applicationService.Accounts.Any())
			{
				ShowViewModel<Accounts.AccountsViewModel>();
				ShowViewModel<Accounts.AddAccountViewModel>();
				return;
			}

			var account = GetDefaultAccount() as BitbucketAccount;
			if (account == null)
			{
				ShowViewModel<Accounts.AccountsViewModel>();
				return;
			}

			if (account.DontRemember)
			{
				ShowViewModel<Accounts.AccountsViewModel>();
				ShowViewModel<Accounts.AddAccountViewModel>(new Accounts.AddAccountViewModel.NavObject() { AttemptedAccountId = account.Id });
				return;
			}

			//Lets login!
			try
			{
				IsLoggingIn = true;

                Uri accountAvatarUri = null;
                var avatarUrl = account.AvatarUrl;
                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    var match = Regex.Match(avatarUrl, @"&s=(\d+)", RegexOptions.IgnoreCase);
                    if (match.Success && match.Groups.Count > 1)
                        avatarUrl = avatarUrl.Replace(match.Groups[0].Value, "&s=128");
                }

                Uri.TryCreate(avatarUrl, UriKind.Absolute, out accountAvatarUri);
                ImageUrl = accountAvatarUri;
                Status = "Logging in as " + account.Username;

				var client = await _loginService.LoginAccount(account);
				_applicationService.ActivateUser(account, client);
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to login successfully: " + e.Message);
				ShowViewModel<Accounts.AccountsViewModel>();
				ShowViewModel<Accounts.AddAccountViewModel>(new Accounts.AddAccountViewModel.NavObject() { AttemptedAccountId = account.Id });
			}
			finally
			{
				IsLoggingIn = false;
			}

		}
    }
}

