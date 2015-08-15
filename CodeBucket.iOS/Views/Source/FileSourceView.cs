using System;
using CodeFramework.iOS.Views;
using MonoTouch.UIKit;
using CodeFramework.Core.ViewModels;
using MonoTouch.Foundation;
using Cirrious.CrossCore;
using CodeFramework.Core.Services;

namespace CodeBucket.iOS.Views.Source
{
	public abstract class FileSourceView : WebView
    {
		private bool _loaded = false;

		public new FileSourceViewModel ViewModel
		{ 
			get { return (FileSourceViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		protected FileSourceView()
			: base(false)
		{
			NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => ShowExtraMenu());
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Stupid but I can't put this in the ViewDidLoad...
			if (!_loaded)
			{
				ViewModel.LoadCommand.Execute(null);
				_loaded = true;
			}

			Title = ViewModel.Title;
		}

		private void ShowExtraMenu()
		{
			var sheet = MonoTouch.Utilities.GetSheet(Title);

			var openButton = sheet.AddButton("Open In".t());
			var shareButton = ViewModel.HtmlUrl != null ? sheet.AddButton("Share".t()) : -1;
			var showButton = ViewModel.HtmlUrl != null ? sheet.AddButton("Show in Bitbucket".t()) : -1;
			var cancelButton = sheet.AddButton("Cancel".t());
			sheet.CancelButtonIndex = cancelButton;
			sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (s, e) => {
                BeginInvokeOnMainThread(() =>
                {
				if (e.ButtonIndex == openButton)
				{
					var ctrl = new UIDocumentInteractionController();
					ctrl.Url = NSUrl.FromFilename(ViewModel.FilePath);
					ctrl.PresentOpenInMenu(NavigationItem.RightBarButtonItem, true);
				}
				else if (e.ButtonIndex == shareButton)
				{
                    Mvx.Resolve<IShareService>().ShareUrl(ViewModel.HtmlUrl);
				}
				else if (e.ButtonIndex == showButton)
				{
					ViewModel.GoToHtmlUrlCommand.Execute(null);
				}
                });
			};

			sheet.ShowInView(this.View);
		}
    }
}

