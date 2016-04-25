using System;

namespace CodeBucket.Core.ViewModels.App
{
    public class SettingsViewModel : BaseViewModel
    {
		public string DefaultStartupViewName
		{
			get { return this.GetApplication().Account.DefaultStartupView; }
		}
    }
}
