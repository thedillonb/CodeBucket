using System;

namespace CodeBucket.Core.ViewModels.App
{
    public class SettingsViewModel : BaseViewModel
    {
		public string DefaultStartupViewName
		{
			get { return this.GetApplication().Account.DefaultStartupView; }
		}

        public ReactiveUI.IReactiveCommand<object> GoToDefaultStartupViewCommand { get; } = ReactiveUI.ReactiveCommand.Create();

        public SettingsViewModel()
        {
            GoToDefaultStartupViewCommand.Subscribe(_ => ShowViewModel<DefaultStartupViewModel>());
        }
    }
}
