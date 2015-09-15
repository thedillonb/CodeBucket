// --------------------------------------------------------------------------------------------------------------------
// <summary>
//    Defines the Setup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;
using Cirrious.CrossCore.IoC;
using Cirrious.MvvmCross.Binding.BindingContext;
using UIKit;
using CodeBucket.Core.ViewModels.App;
using CodeBucket.Elements;
using CodeBucket.Core.ViewModels;

namespace CodeBucket
{
	using Cirrious.MvvmCross.Touch.Platform;
	using Cirrious.MvvmCross.Touch.Views.Presenters;
	using Cirrious.MvvmCross.ViewModels;

	/// <summary>
	///    Defines the Setup type.
	/// </summary>
	public class Setup : MvxTouchSetup
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Setup"/> class.
		/// </summary>
		/// <param name="applicationDelegate">The application delegate.</param>
		/// <param name="presenter">The presenter.</param>
		public Setup(MvxApplicationDelegate applicationDelegate, IMvxTouchViewPresenter presenter)
			: base(applicationDelegate, presenter)
		{
		}

		protected override Assembly[] GetViewModelAssemblies()
		{
			var list = new List<Assembly>();
			list.AddRange(base.GetViewModelAssemblies());
			list.Add(typeof(StartupViewModel).Assembly);
			return list.ToArray();
		}

		protected override void FillBindingNames(IMvxBindingNameRegistry obj)
		{
			base.FillBindingNames(obj);
			obj.AddOrOverwrite(typeof(StyledStringElement), "Tapped");
			obj.AddOrOverwrite(typeof(UISegmentedControl), "ValueChanged");
		}

		/// <summary>
		/// Creates the app.
		/// </summary>
		/// <returns>An instance of IMvxApplication</returns>
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