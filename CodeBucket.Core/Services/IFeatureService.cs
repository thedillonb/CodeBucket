using System.Threading.Tasks;

namespace CodeBucket.Core.Services
{
    public interface IFeaturesService
    {
        bool IsProEnabled { get; }

        void ActivateProDirect();

        Task ActivatePro();

        Task RestorePro();
    }
}
