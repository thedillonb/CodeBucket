using CodeFramework.Core.ViewModels;
using Cirrious.CrossCore;
using CodeBucket.Core.Services;

public static class ViewModelExtensions
{
	public static IApplicationService GetApplication(this BaseViewModel vm)
    {
		return Mvx.Resolve<IApplicationService>();
    }
}

