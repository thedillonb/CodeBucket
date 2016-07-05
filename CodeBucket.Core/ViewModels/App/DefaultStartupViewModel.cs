using CodeBucket.Core.Services;
using System;
using CodeBucket.Core.Utils;
using System.Linq;
using ReactiveUI;
using System.Reactive.Linq;
using Splat;
using System.Collections.Generic;

namespace CodeBucket.Core.ViewModels.App
{
    public class DefaultStartupViewModel : BaseViewModel
    {
        private string _selectedStartupView;
        public string SelectedStartupView
        {
            get { return _selectedStartupView; }
            set { this.RaiseAndSetIfChanged(ref _selectedStartupView, value); }
        }

        private IList<string> _startupViews;
        public IList<string> StartupViews
        {
            get { return _startupViews; }
            private set { this.RaiseAndSetIfChanged(ref _startupViews, value); }
        }

		public DefaultStartupViewModel(IAccountsService accountsService = null)
		{
            accountsService = accountsService ?? Locator.Current.GetService<IAccountsService>();

            var props = from p in typeof(MenuViewModel).GetProperties()
                        let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
                        where attr.Length == 1
                        select attr[0] as PotentialStartupViewAttribute;

            SelectedStartupView = accountsService.ActiveAccount.DefaultStartupView;
            StartupViews = props.Select(x => x.Name).ToList();

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

