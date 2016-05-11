using System;
using CodeBucket.Core.Data;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Core.ViewModels.App
{
    public class SettingsViewModel : BaseViewModel
    {
        readonly IApplicationService _applicationService;

        public string DefaultStartupViewName => _applicationService.Account.DefaultStartupView;

        public BitbucketAccount Account => _applicationService.Account;

        public void SetAccount(Action<BitbucketAccount> account)
        {
            account(_applicationService.Account);
            _applicationService.Accounts.Update(_applicationService.Account);
        }

        public SettingsViewModel(IApplicationService applicationService = null)
        {
            _applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            Title = "Settings";
        }
    }
}
