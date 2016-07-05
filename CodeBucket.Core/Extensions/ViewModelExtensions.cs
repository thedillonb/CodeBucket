using CodeBucket.Core.Services;
using CodeBucket.Core.ViewModels;
using Splat;

public static class ViewModelExtensions
{
	public static IApplicationService GetApplication(this BaseViewModel vm)
    {
		return Locator.Current.GetService<IApplicationService>();
    }
}

