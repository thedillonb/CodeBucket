using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using System.Linq;

namespace CodeBucket.Core.ViewModels.App
{
    public class SettingsViewModel : BaseViewModel
    {
		public string DefaultStartupViewName
		{
			get { return this.GetApplication().Account.DefaultStartupView; }
		}

		public ICommand GoToDefaultStartupViewCommand
		{
			get { return new MvxCommand(() => ShowViewModel<DefaultStartupViewModel>()); }
		}
    }
}
