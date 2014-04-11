using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using CodeFramework.Core.ViewModels;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class AddAccountViewModel : BaseViewModel 
    {
		private BitbucketAccount _attemptedAccount;
        private readonly IApplicationService _application;
        private readonly ILoginService _loginService;
        private string _username;
        private string _password;
        private string _domain;
        private bool _isLoggingIn;

        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            set { _isLoggingIn = value; RaisePropertyChanged(() => IsLoggingIn); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; RaisePropertyChanged(() => Username); }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; RaisePropertyChanged(() => Password); }
        }

        public string TwoFactor { get; set; }

        public ICommand LoginCommand
        {
            get { return new MvxCommand(Login, CanLogin);}
        }

        public AddAccountViewModel(IApplicationService application, ILoginService loginService)
        {
            _application = application;
            _loginService = loginService;
        }

        public void Init(NavObject navObject)
        {
			if (navObject.AttemptedAccountId >= 0)
				_attemptedAccount = this.GetApplication().Accounts.Find(navObject.AttemptedAccountId) as BitbucketAccount;

            if (_attemptedAccount != null)
            {
                Username = _attemptedAccount.Username;
            }
        }

        private bool CanLogin()
        {
            if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password))
                return false;
            return true;
        }

        private async void Login()
        {
            try
            {
                IsLoggingIn = true;
				var loginData = await Task.Run(() => _loginService.Authenticate(Username, Password, _attemptedAccount));
				var client = await Task.Run(() => _loginService.LoginAccount(loginData.Account));
				_application.ActivateUser(loginData.Account, client);
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
			public int AttemptedAccountId { get; set; }

			public NavObject()
			{
				AttemptedAccountId = int.MinValue;
			}
        }
    }
}
