using System.Collections.Generic;
using System.Reflection;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.Core.ViewModels;
using MvvmCross.iOS.Platform;
using MvvmCross.iOS.Views.Presenters;
using MvvmCross.Platform.IoC;
using MvvmCross.Core.ViewModels;

namespace CodeBucket
{

	public class Setup : MvxIosSetup
	{
		public Setup(MvxApplicationDelegate applicationDelegate, IMvxIosViewPresenter presenter)
			: base(applicationDelegate, presenter)
		{
		}

		protected override IEnumerable<Assembly> GetViewModelAssemblies()
		{
			var list = new List<Assembly>();
			list.AddRange(base.GetViewModelAssemblies());
			list.Add(typeof(StartupViewModel).Assembly);
			return list.ToArray();
		}

		protected override IMvxApplication CreateApp()
		{
			this.CreatableTypes(typeof(BaseViewModel).Assembly)
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			this.CreatableTypes()
				.EndingWith("Service")
				.AsInterfaces()
				.RegisterAsLazySingleton();

			return new Core.App();
		}
	}
}