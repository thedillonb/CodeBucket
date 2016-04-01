using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Threading.Tasks;
using CodeBucket.Core.ViewModels.Accounts;
using BitbucketSharp;
using BitbucketSharp.Models;
using CodeBucket.Core.Utils;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.App
{
    public class StartupViewModel : BaseViewModel
    {
        private readonly IApplicationService _applicationService;
        private readonly IAccountsService _accountsService;
        private bool _isLoggingIn;
        private string _status;
        private Avatar _avatar;

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            private set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
        }

        public string Status
        {
            get { return _status; }
            private set { this.RaiseAndSetIfChanged(ref _status, value); }
        }

        public Avatar Avatar
        {
            get { return _avatar; }
            private set { this.RaiseAndSetIfChanged(ref _avatar, value); }
        }

        public ReactiveUI.ReactiveCommand<object> GoToMenuCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.ReactiveCommand<object> GoToAccountsCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ReactiveUI.ReactiveCommand<object> GoToLoginCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public ICommand StartupCommand
        {
            get { return new MvxAsyncCommand(Startup); }
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
                GoToLoginCommand.Execute(null);
				return;
			}

			var account = GetDefaultAccount();
			if (account == null)
			{
                GoToAccountsCommand.Execute(null);
				return;
			}

            if (string.IsNullOrEmpty(account.Token) || string.IsNullOrEmpty(account.RefreshToken))
            {
                await AlertService.Alert("Welcome!", "CodeBucket is now OAuth compliant!\n\nFor your security, " +
                "you will now be prompted to login to Bitbucket via their OAuth portal. This will swap out your credentials" +
                " for an OAuth token you may revoke at any time!");
                GoToLoginCommand.Execute(null);
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
                    GoToLoginCommand.Execute(null);
                    return;
                }

                account.RefreshToken = ret.RefreshToken;
                account.Token = ret.AccessToken;
                _accountsService.Update(account);

                await AttemptLogin(account);

                GoToMenuCommand.Execute(null);
            }
            catch (Exception e)
            {
                DisplayAlert("Unable to login successfully: " + e.Message).FireAndForget();
                GoToAccountsCommand.Execute(null);
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

            if (Uri.TryCreate(avatarUrl, UriKind.Absolute, out accountAvatarUri))
                Avatar = new Avatar(accountAvatarUri.AbsoluteUri);

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

