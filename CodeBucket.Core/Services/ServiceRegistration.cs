using Splat;

namespace CodeBucket.Core.Services
{
    public static class ServiceRegistration
    {
        public static void Register()   
        {
            var locator = Locator.CurrentMutable;

            locator.RegisterLazySingleton(() => new AccountsService(
                locator.GetService<IDefaultValueService>(), 
                locator.GetService<IAccountPreferencesService>()), 
                                          typeof(IAccountsService));

            locator.RegisterLazySingleton(() => new ApplicationService(
                locator.GetService<IAccountsService>(),
                locator.GetService<ILoadingIndicatorService>()),
                                          typeof(IApplicationService));
        }
    }
}

