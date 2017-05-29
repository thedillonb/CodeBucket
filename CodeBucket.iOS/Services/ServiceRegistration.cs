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

            locator.RegisterLazySingleton(() => new ActionMenuService(), typeof(IActionMenuService));

            locator.RegisterLazySingleton(() => new AlertDialogService(), typeof(IAlertDialogService));

            locator.RegisterLazySingleton(() => new DefaultValueService(), typeof(IDefaultValueService));

            //locator.RegisterLazySingleton(() => new MarkdownService(), typeof(IMarkdownService));
            locator.RegisterConstant(new MarkdownService(), typeof(IMarkdownService));

            //locator.RegisterLazySingleton(() => new ViewLocatorService(), typeof(IViewLocatorService));

            locator.RegisterConstant(new ViewLocatorService(), typeof(IViewLocatorService));

            locator.RegisterLazySingleton(() => new LoadingIndicatorService(), typeof(ILoadingIndicatorService));

            locator.RegisterLazySingleton(() => new DiffService(), typeof(IDiffService));

            locator.RegisterLazySingleton(() => new FeaturesService(), typeof(IFeaturesService));

            locator.RegisterLazySingleton(() => new InAppPurchaseService(), typeof(IInAppPurchaseService));
        }
    }
}
