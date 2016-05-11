using System;
using CodeBucket.Core.Services;
using System.Threading.Tasks;
using BitbucketSharp.Models;
using CodeBucket.Core.Data;
using BitbucketSharp;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using CodeBucket.Core.Messages;
using Splat;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class LoginViewModel : ReactiveObject
    {
		public const string ClientId = "gtAAHvjnAp9W45Gk6P";
		public const string ClientSecret = "bRYpfaTt7ZwsCkpu2DPehfDNPLKGNJ5z";
        private readonly IAccountsService _accountsService;
        private readonly IApplicationService _applicationService;

		private bool _isLoggingIn;
		public bool IsLoggingIn
		{
			get { return _isLoggingIn; }
            set { this.RaiseAndSetIfChanged(ref _isLoggingIn, value); }
		}

        private string _code;
        public string Code
        {
            get { return _code; }
            set { this.RaiseAndSetIfChanged(ref _code, value); }
        }

		public string LoginUrl
		{
			get
			{
                return string.Format("https://bitbucket.org/site/oauth2/authorize?client_id={0}&response_type=code", ClientId);
			}
		}

        public ReactiveCommand<Unit> LoginCommand { get; }

        public LoginViewModel(
            IApplicationService applicationService = null, 
            IAccountsService accountsService = null)
		{
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();

            LoginCommand = ReactiveCommand.CreateAsyncTask(
                this.WhenAnyValue(x => x.Code).Select(x => x != null),
                t => Login());

            LoginCommand.Subscribe(x => MessageBus.Current.SendMessage(new LogoutMessage()));
		}

        private async Task Login()
		{
			try
			{
				IsLoggingIn = true;
                var ret = await Client.GetAuthorizationCode(ClientId, ClientSecret, Code);
                var client = Client.WithBearerAuthentication(ret.AccessToken);
                var user = await client.Users.GetUser();

                var account = _accountsService.Find(user.Username);
                if (account == null)
                {
                    account = new BitbucketAccount
                    {
                        Username = user.Username,
                        AvatarUrl = user.Links.Avatar.Href,
                        RefreshToken = ret.RefreshToken,
                        Token = ret.AccessToken
                    };
                    _accountsService.Insert(account);
                }
                else
                {
                    account.RefreshToken = ret.RefreshToken;
                    account.Token = ret.AccessToken;
                    account.AvatarUrl = user.Links.Avatar.Href;
                    _accountsService.Update(account);
                }

                _applicationService.ActivateUser(account, client);
			}
			finally
			{
				IsLoggingIn = false;
			}
		}
    }
}

