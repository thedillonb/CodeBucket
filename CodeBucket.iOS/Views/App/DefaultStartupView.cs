using System;
using UIKit;
using System.Linq;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.DialogElements;
using CodeBucket.Core.Services;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.Views.App
{
    public class DefaultStartupView : DialogViewController
    {
        public DefaultStartupViewModel ViewModel { get; }

		public DefaultStartupView()
            : base(UITableViewStyle.Plain)
        {
			Title = "Default Startup View";
			EnableSearch = false;
            ViewModel = new DefaultStartupViewModel(MvvmCross.Platform.Mvx.Resolve<IAccountsService>());
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.StartupViews.ChangedObservable().Subscribe(views =>
            {
                var elements = views.Select(x =>
                {
                    var e = new StringElement(x);
                    e.Clicked.Subscribe(_ => ViewModel.SelectedStartupView = x);
                    e.Accessory = string.Equals(ViewModel.SelectedStartupView, x)
                        ? UITableViewCellAccessory.Checkmark
                        : UITableViewCellAccessory.None;
                    return e;  
                });

                Root.Reset(new Section { elements });
            });

            ViewModel.WhenAnyValue(x => x.SelectedStartupView).Skip(1).Subscribe(x =>
			{
                foreach (var m in Root[0].Elements.Cast<StringElement>())
					m.Accessory = (string.Equals(m.Caption, x)) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
                NavigationController.PopViewController(true);
			});
		}
    }
}

