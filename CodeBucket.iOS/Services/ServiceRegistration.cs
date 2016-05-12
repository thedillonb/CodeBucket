using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Services
{
    public static class ServiceRegistration
    {
        public static void Register()
        {
            Core.Services.ServiceRegistration.Register();

            var locator = Locator.CurrentMutable;

            locator.RegisterLazySingleton(() => new AccountPreferencesService(), typeof(IAccountPreferencesService));

            locator.RegisterLazySingleton(() => new ActionMenuService(), typeof(IActionMenuService));

            locator.RegisterLazySingleton(() => new AlertDialogService(), typeof(IAlertDialogService));

            locator.RegisterLazySingleton(() => new DefaultValueService(), typeof(IDefaultValueService));

            locator.RegisterLazySingleton(() => new MarkdownService(), typeof(IMarkdownService));

            locator.RegisterLazySingleton(() => new ViewLocatorService(), typeof(IViewLocatorService));
        }
    }
}

