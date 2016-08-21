using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.ViewModels.App
{
    public class SettingsViewModel : BaseViewModel
    {
        readonly IApplicationService _applicationService;
        readonly IAccountsService _accountsService;

        public string DefaultStartupViewName => _applicationService.Account.DefaultStartupView;

        public Account Account => _applicationService.Account;

        public void SetAccount(Action<Account> account)
        {
            account(_applicationService.Account);
            _accountsService.Save(_applicationService.Account).ToBackground();
        }

        public SettingsViewModel(
            IApplicationService applicationService = null,
            IAccountsService accountsService = null)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();
            _accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();

            Title = "Settings";
        }
    }
}
