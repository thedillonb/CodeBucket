using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace CodeBucket.Core.ViewModels.App
{
    public class StartupViewModel : BaseViewModel
    {
        private readonly ILoginService _loginService;
        private readonly IApplicationService _applicationService;
        private bool _isLoggingIn;
        private string _status;
        private Uri _imageUrl;

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            protected set
            {
                _isLoggingIn = value;
                RaisePropertyChanged(() => IsLoggingIn);
            }
        }

        public string Status
        {
            get { return _status; }
            protected set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public Uri ImageUrl
        {
            get { return _imageUrl; }
            protected set
            {
                _imageUrl = value;
                RaisePropertyChanged(() => ImageUrl);
            }
        }

        public ICommand StartupCommand
        {
            get { return new MvxCommand(Startup);}
        }

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        protected IAccount GetDefaultAccount()
        {
            var accounts = GetService<IAccountsService>();
            return accounts.GetDefault();
        }

		public StartupViewModel(ILoginService loginService, IApplicationService applicationService)
		{
			_loginService = loginService;
			_applicationService = applicationService;
		}

		protected async void Startup()
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

