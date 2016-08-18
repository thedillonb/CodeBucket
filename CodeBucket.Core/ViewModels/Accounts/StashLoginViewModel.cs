using System;
using System.Threading.Tasks;
using CodeBucket.Core.Services;
using CodeBucket.Core.Messages;
using System.Reactive;
using ReactiveUI;
using System.Reactive.Linq;
using Splat;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class StashLoginViewModel : BaseViewModel
    {
        private readonly IApplicationService _application;

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
        }

        private string _username;
        public string Username
        {
            get { return _username; }
            set { this.RaiseAndSetIfChanged(ref _username, value); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { this.RaiseAndSetIfChanged(ref _password, value); }
        }

        private string _domain;
        public string Domain
        {
            get { return _domain; }
            set { this.RaiseAndSetIfChanged(ref _domain, value); }
        }

        public string TwoFactor { get; set; }

        public IReactiveCommand<Unit> LoginCommand { get; }

        public StashLoginViewModel(IApplicationService application = null)
        {
            _application = application ?? Locator.Current.GetService<IApplicationService>();

            LoginCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.Username, x => x.Password)
                .Select(x => !string.IsNullOrEmpty(x.Item1) && !string.IsNullOrEmpty(x.Item2)),
                _ => Login());
        }

        private async Task Login()
        {
            var apiUrl = Domain;
            if (apiUrl != null)
            {
                if (!apiUrl.StartsWith("http://") && !apiUrl.StartsWith("https://"))
                    apiUrl = "https://" + apiUrl;
                if (!apiUrl.EndsWith("/"))
                    apiUrl += "/";
                if (!apiUrl.Contains("/api/"))
                    apiUrl += "api/v3/";
            }

            try
            {
                IsLoggingIn = true;
                //var account = await _loginFactory.LoginWithBasic(apiUrl, Username, Password, TwoFactor);
                //var client = await _loginFactory.LoginAccount(account);
                //_application.ActivateUser(account, client);
                MessageBus.Current.SendMessage(new LogoutMessage());
            }
            catch (Exception)
            {
                TwoFactor = null;
                throw;
            }
            finally
            {
                IsLoggingIn = false;
            }
        }
    }
}