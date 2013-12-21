using CodeFramework.Core.ViewModels.App;
using CodeFramework.Core.Services;

namespace CodeBucket.Core.ViewModels.App
{
	public class DefaultStartupViewModel : BaseDefaultStartupViewModel
    {
		public DefaultStartupViewModel(IAccountsService accountsService)
			: base(accountsService, typeof(MenuViewModel))
		{
		}
    }
}

