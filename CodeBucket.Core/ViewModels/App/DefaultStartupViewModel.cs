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

        public DefaultStartupViewModel(IApplicationService applicationService = null)
		{
            applicationService = applicationService ?? Locator.Current.GetService<IApplicationService>();

            var props = from p in typeof(MenuViewModel).GetProperties()
                        let attr = p.GetCustomAttributes(typeof(PotentialStartupViewAttribute), true)
                        where attr.Length == 1
                        select attr[0] as PotentialStartupViewAttribute;

            SelectedStartupView = applicationService.Account.DefaultStartupView;
            StartupViews = props.Select(x => x.Name).ToList();

            this.WhenAnyValue(x => x.SelectedStartupView)
                .Where(x => x != applicationService.Account.DefaultStartupView)
                .Subscribe(x =>
                {
                    applicationService.Account.DefaultStartupView = x;
                    applicationService.SaveAccount().ToBackground();
                });
		}
    }
}

