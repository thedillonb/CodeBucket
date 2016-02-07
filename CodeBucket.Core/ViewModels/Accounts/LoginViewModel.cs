using System;
using CodeBucket.Core.Services;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;
using System.Threading.Tasks;
using BitbucketSharp.Models;
using CodeBucket.Core.Data;
using BitbucketSharp;
using CodeBucket.Core.ViewModels.App;

namespace CodeBucket.Core.ViewModels.Accounts
{
	public class LoginViewModel : BaseViewModel
    {
		public const string ClientId = "gtAAHvjnAp9W45Gk6P";
		public const string ClientSecret = "bRYpfaTt7ZwsCkpu2DPehfDNPLKGNJ5z";
        private readonly IAccountsService _accountsService;

		private bool _isLoggingIn;
		public bool IsLoggingIn
		{
			get { return _isLoggingIn; }
			set
			{
				_isLoggingIn = value;
				RaisePropertyChanged(() => IsLoggingIn);
			}
		}

		public string LoginUrl
		{
			get
			{
                return string.Format("https://bitbucket.org/site/oauth2/authorize?client_id={0}&response_type=code", LoginViewModel.ClientId);
			}
		}

		public ICommand GoBackCommand
		{
			get { return new MvxCommand(() => ChangePresentation(new MvxClosePresentationHint(this))); }
		}

        public LoginViewModel(IAccountsService accountsService)
		{
            _accountsService = accountsService;
		}

		public async void Login(string code)
		{
			try
			{
				IsLoggingIn = true;
                var ret = await Task.Run(() => Client.GetAuthorizationCode(ClientId, ClientSecret, code));
                var data = await Task.Run(() => {
                    UsersModel u;
                    var c = Client.BearerLogin(ret.AccessToken, out u);
                    return Tuple.Create(c, u);
                });

                var bitbucketClient = data.Item1;
                var usersModel = data.Item2;

                var account = _accountsService.Find(usersModel.User.Username);
                if (account == null)
                {
                    account = new BitbucketAccount()
                    {
                        Username = usersModel.User.Username,
                        AvatarUrl = usersModel.User.Avatar,
                        RefreshToken = ret.RefreshToken,
                        Token = ret.AccessToken
                    };
                    _accountsService.Insert(account);
                }
                else
                {
                    account.RefreshToken = ret.RefreshToken;
                    account.Token = ret.AccessToken;
                    account.AvatarUrl = usersModel.User.Avatar;
                    _accountsService.Update(account);
                }

                this.GetApplication().ActivateUser(account, bitbucketClient);
                ShowViewModel<StartupViewModel>();
			}
			catch (Exception e)
			{
                DisplayAlert("Unable to login: " + e.Message);
			}
			finally
			{
				IsLoggingIn = false;
			}
		}
    }
}

