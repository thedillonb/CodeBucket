using Splat;

namespace CodeBucket.Core.Services
{
    public static class ServiceRegistration
    {
        public static void Register()
        {
            var locator = Locator.CurrentMutable;

            locator.RegisterLazySingleton(() => new AccountsService(), typeof(IAccountsService));

            locator.RegisterLazySingleton(() => new ApplicationService(
                locator.GetService<IAccountsService>(),
                locator.GetService<IDefaultValueService>()),
                typeof(IApplicationService));

            locator.RegisterLazySingleton(() => new MessageService(),
                typeof(IMessageService));
        }
    }
}