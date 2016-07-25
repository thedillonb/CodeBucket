using System.Threading.Tasks;
using CodeBucket.Core.Services;
using Splat;

namespace CodeBucket.Services
{
    public class FeaturesService : IFeaturesService
    {
        private readonly IDefaultValueService _defaultValueService;
        private readonly IInAppPurchaseService _inAppPurchaseService;

        public const string ProEdition = "com.dillonbuchanan.codebucket.pro";

        public FeaturesService(IDefaultValueService defaultValueService = null, IInAppPurchaseService inAppPurchaseService = null)
        {
            _defaultValueService = defaultValueService ?? Locator.Current.GetService<IDefaultValueService>();
            _inAppPurchaseService = inAppPurchaseService ?? Locator.Current.GetService<IInAppPurchaseService>();
        }

        public bool IsProEnabled
        {
            get
            {
                return IsActivated(ProEdition);
            }
        }

        public Task ActivatePro()
        {
            return _inAppPurchaseService.PurchaseProduct(ProEdition);
        }

        public void ActivateProDirect()
        {
            _defaultValueService.Set(ProEdition, true);
        }

        public async Task RestorePro()
        {
            await _inAppPurchaseService.Restore();
        }

        private bool IsActivated(string id)
        {
            bool value;
            return _defaultValueService.TryGet(id, out value) && value;
        }

    }
}