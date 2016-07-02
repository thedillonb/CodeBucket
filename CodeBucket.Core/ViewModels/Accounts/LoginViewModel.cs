using System;
using CodeBucket.Core.Services;
using System.Threading.Tasks;
using CodeBucket.Client.Models;
using CodeBucket.Core.Data;
using CodeBucket.Client;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using CodeBucket.Core.Messages;

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
                return string.Format("https://bitbucket.org/site/oauth2/authorize?client_id={0}&response_type=code", LoginViewModel.ClientId);
			}
		}

        public ReactiveCommand<Unit> LoginCommand { get; }

        public LoginViewModel(IApplicationService applicationService, IAccountsService accountsService)
		{
            _applicationService = applicationService;
            _accountsService = accountsService;

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
                var ret = await BitbucketClient.GetAuthorizationCode(ClientId, ClientSecret, Code);
                var bitbucketClient = BitbucketClient.WithBearerAuthentication(ret.AccessToken);
                var usersModel = await bitbucketClient.Users.GetCurrent();

                var account = _accountsService.Find(usersModel.User.Username);
                if (account == null)
                {
                    account = new BitbucketAccount
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

                _applicationService.ActivateUser(account, bitbucketClient);
			}
			finally
			{
				IsLoggingIn = false;
			}
		}
    }
}

