using System;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;
using System.Reactive;
using CodeBucket.Core.Messages;
using Splat;

namespace CodeBucket.Core.ViewModels.Accounts
{
    public class LoginViewModel : ReactiveObject
    {
		public string LoginUrl
		{
			get
			{
                return string.Format(
                    "https://bitbucket.org/site/oauth2/authorize?client_id={0}&response_type=code",
                    Secrets.ClientId);
			}
		}

        public ReactiveCommand<string, Unit> LoginCommand { get; }

        public LoginViewModel(
            IApplicationService applicationService = null,
            IAlertDialogService alertDialogService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            alertDialogService = alertDialogService ?? Locator.Current.GetService<IAlertDialogService>();

            LoginCommand = ReactiveCommand.CreateFromTask<string>(applicationService.Login);
            LoginCommand.Subscribe(x => MessageBus.Current.SendMessage(new LogoutMessage()));
            LoginCommand.ThrownExceptions
                        .Select(ex => alertDialogService.Alert("Error", $"Unable to successfully login. {ex.Message}"))
                        .Subscribe(x => x.ToBackground());
		}
    }
}

