using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace CodeBucket.Core.ViewModels.Accounts
{
	public class LoginViewModel : BaseViewModel
    {
		public const string ClientId = "gtAAHvjnAp9W45Gk6P";
		public const string ClientSecret = "bRYpfaTt7ZwsCkpu2DPehfDNPLKGNJ5z";
		public static readonly string RedirectUri = "http://dillonbuchanan.com/";
		private static readonly string RequestTokenUrl = "https://bitbucket.org/api/1.0/oauth/request_token";
		private readonly ILoginService _loginService;

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
				return string.Format("/login/oauth/authorize?client_id={0}&redirect_uri={1}&scope={2}", 
					LoginViewModel.ClientId, Uri.EscapeUriString(LoginViewModel.RedirectUri), Uri.EscapeUriString("user,public_repo,repo,notifications,gist"));
			}
		}

		public BitbucketAccount AttemptedAccount { get; private set; }

		public ICommand GoToOldLoginWaysCommand
		{
			get { return new MvxCommand(() => ShowViewModel<AddAccountViewModel>(new AddAccountViewModel.NavObject())); }
		}

		public ICommand GoBackCommand
		{
			get { return new MvxCommand(() => ChangePresentation(new MvxClosePresentationHint(this))); }
		}

		public LoginViewModel(ILoginService loginService)
		{
			_loginService = loginService;
		}

		public void Init(NavObject navObject)
		{
			if (navObject.AttemptedAccountId >= 0)
			{
				AttemptedAccount = this.GetApplication().Accounts.Find(navObject.AttemptedAccountId) as BitbucketAccount;
			}
		}

		public async void Login(string code)
		{
			try
			{
				IsLoggingIn = true;
				var account = AttemptedAccount;
				var data = await _loginService.LoginWithToken(ClientId, ClientSecret, code, RedirectUri, string.Empty, string.Empty, account);
				this.GetApplication().ActivateUser(data.Account, data.Client);
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

		public class NavObject
		{
			public string Username { get; set; }
			public int AttemptedAccountId { get; set; }

			public NavObject()
			{
				AttemptedAccountId = int.MinValue;
			}

			public static NavObject CreateDontRemember(BitbucketAccount account)
			{
				return new NavObject
				{ 
					Username = account.Username,
					AttemptedAccountId = account.Id
				};
			}
		}
    }
}

