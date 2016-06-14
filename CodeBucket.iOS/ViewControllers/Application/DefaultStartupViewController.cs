using System;
using UIKit;
using System.Linq;
using CodeBucket.ViewControllers;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.DialogElements;
using ReactiveUI;
using System.Reactive.Linq;

namespace CodeBucket.ViewController.Application
{
    public class DefaultStartupViewController : DialogViewController
    {
        public DefaultStartupViewModel ViewModel { get; } = new DefaultStartupViewModel();

		public DefaultStartupViewController()
            : base(UITableViewStyle.Plain)
        {
			Title = "Default Startup View";
			EnableSearch = false;
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.StartupViews.ChangedObservable().Subscribe(views =>
            {
                var elements = views.Select(x =>
                {
                    var e = new CheckElement(x);
                    e.CheckedChanged.Subscribe(_ => ViewModel.SelectedStartupView = x);
                    e.Checked = string.Equals(ViewModel.SelectedStartupView, x);
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

