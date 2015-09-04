using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Accounts;
using BitbucketSharp;
using BitbucketSharp.Models;

namespace CodeBucket.Core.ViewModels.App
{
    public class StartupViewModel : BaseViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IAccountsService _accountsService;
        private bool _isLoggingIn;
        private string _status;
        private Uri _imageUrl;

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            private set
            {
                _isLoggingIn = value;
                RaisePropertyChanged(() => IsLoggingIn);
            }
        }

        public string Status
        {
            get { return _status; }
            private set
            {
                _status = value;
                RaisePropertyChanged(() => Status);
            }
        }

        public Uri ImageUrl
        {
            get { return _imageUrl; }
            private set
            {
                _imageUrl = value;
                RaisePropertyChanged(() => ImageUrl);
            }
        }

        public ICommand StartupCommand
        {
            get { return new MvxCommand(() => Startup());}
        }

        /// <summary>
        /// Gets the default account. If there is not one assigned it will pick the first in the account list.
        /// If there isn't one, it'll just return null.
        /// </summary>
        /// <returns>The default account.</returns>
        protected BitbucketAccount GetDefaultAccount()
        {
            return _accountsService.GetDefault();
        }

		public StartupViewModel(IAccountsService accountsService, IApplicationService applicationService)
		{
            _accountsService = accountsService;
			_applicationService = applicationService;
		}

        protected async Task Startup()
		{
			if (!_applicationService.Accounts.Any())
			{
                ShowViewModel<LoginViewModel>();
				return;
			}

			var account = GetDefaultAccount();
			if (account == null)
			{
				ShowViewModel<AccountsViewModel>();
				return;
			}

            if (string.IsNullOrEmpty(account.Token) || string.IsNullOrEmpty(account.RefreshToken))
            {
                await AlertService.Alert("Welcome!", "CodeBucket is now OAuth compliant!\n\nFor your security, " +
                "you will now be prompted to login to Bitbucket via their OAuth portal. This will swap out your credentials" +
                " for an OAuth token you may revoke at any time!");
                ShowViewModel<LoginViewModel>();
                return;
            }

            try
            {
                IsLoggingIn = true;
                Status = "Logging in as " + account.Username;

                var ret = await Task.Run(() => Client.RefreshToken(LoginViewModel.ClientId, LoginViewModel.ClientSecret, account.RefreshToken));
                if (ret == null)
                {
                    await DisplayAlert("Unable to refresh OAuth token. Please login again.");
                    ShowViewModel<AccountsViewModel>();
                    ShowViewModel<LoginViewModel>();
                    return;
                }

                account.RefreshToken = ret.RefreshToken;
                account.Token = ret.AccessToken;
                _accountsService.Update(account);

                await AttemptLogin(account);

                ShowViewModel<MenuViewModel>();
            }
            catch (Exception e)
            {
                DisplayAlert("Unable to login successfully: " + e.Message);
                ShowViewModel<AccountsViewModel>();
            }
            finally
            {
                IsLoggingIn = false;
            }
		}

        private async Task AttemptLogin(BitbucketAccount account)
        {
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

            var client = await LoginAccount(account);
            _applicationService.ActivateUser(account, client);
        }

        public async Task<Client> LoginAccount(BitbucketAccount account)
        {
            //Create the client
            UsersModel userInfo = null;
            var client = await Task.Run(() => Client.BearerLogin(account.Token, out userInfo));
            account.Username = userInfo.User.Username;
            account.AvatarUrl = userInfo.User.Avatar.Replace("/avatar/32", "/avatar/64");
            _accountsService.Update(account);
            return client;
        }
    }
}

