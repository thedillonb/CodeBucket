namespace CodeBucket.Core.ViewModels
{
    public interface IViewModel
    {
    }

    public interface IBaseViewModel : IProvidesTitle, IRoutingViewModel, IViewModel
    {
    }
}

