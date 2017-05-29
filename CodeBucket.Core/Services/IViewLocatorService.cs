using ReactiveUI;

namespace CodeBucket.Core.Services
{
    public interface IViewLocatorService
    {
        IViewFor GetView(object viewModel);
    }
}
