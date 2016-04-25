using CodeBucket.Core.Services;
using System;
using CodeBucket.Core.Utils;
using System.Linq;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Core.ViewModels.App
{
    public class DefaultStartupViewModel : ReactiveObject
    {
        public IReadOnlyReactiveList<string> StartupViews { get; }

        private string _selectedStartupView;
        public string SelectedStartupView
        {
            get { return _selectedStartupView; }
            set { this.RaiseAndSetIfChanged(ref _selectedStartupView, value); }
        }

		public DefaultStartupViewModel(IAccountsService accountsService)
		{
            var props = from p in typeof(MenuViewModel).GetProperties()
                        let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
                        where attr.Length == 1
                        select attr[0] as PotentialStartupViewAttribute;

            SelectedStartupView = accountsService.ActiveAccount.DefaultStartupView;
            StartupViews = new ReactiveList<string>(props.Select(x => x.Name));

            this.WhenAnyValue(x => x.SelectedStartupView)
                .Where(x => x != accountsService.ActiveAccount.DefaultStartupView)
                .Subscribe(x =>
                {
                    accountsService.ActiveAccount.DefaultStartupView = x;
                    accountsService.Update(accountsService.ActiveAccount);
                });
		}
    }
}

