using MvvmCross.Platform;
using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels;

public static class ViewModelExtensions
{
	public static IApplicationService GetApplication(this BaseViewModel vm)
    {
		return Mvx.Resolve<IApplicationService>();
    }
}

