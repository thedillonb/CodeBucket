using System;
using CodeFramework.iOS.Views;
using CodeBucket.Core.ViewModels.Wiki;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace CodeBucket.iOS
{
	public class WikiView : WebView
    {
		private readonly UIBarButtonItem _editButton;
		private bool _loaded = false;

		public new WikiViewModel ViewModel
		{
			get { return (WikiViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public WikiView()
			: base(true, true)
        {
			_editButton = new UIBarButtonItem(UIBarButtonSystemItem.Edit, (s, e) => HandleEditButton()) { Enabled = false };
        }

        private void HandleEditButton()
        {
//            try
//            {
//                var page = CurrentWikiPage(Web.Request);
//                var wiki = Application.Client.Users[_user].Repositories[_slug].Wikis[page].GetInfo();
//
//
//                var composer = new Composer { Title = "Edit ".t() + Title, Text = wiki.Data, ActionButtonText = "Save".t() };
//                composer.NewComment(this, () => {
//                    var text = composer.Text;
//
//                    composer.DoWork(() => {
//                        Application.Client.Users[_user].Repositories[_slug].Wikis[page].Update(text, Uri.UnescapeDataString("/" + page));
//                        
//                        InvokeOnMainThread(() => {
//                            composer.CloseComposer();
//                            Refresh();
//                        });
//                    }, ex =>
//                    {
//                        Utilities.ShowAlert("Unable to update page!", ex.Message);
//                        composer.EnableSendButton = true;
//                    });
//                });
//            }
//            catch (Exception e)
//            {
//                Utilities.ShowAlert("Error", e.Message);
//            }
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
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			ViewModel.Bind(x => x.ContentUrl, x => InvokeOnMainThread(() => Web.LoadRequest(new NSUrlRequest(new NSUrl(x)))));
		}

		protected override void OnLoadStarted(object sender, EventArgs e)
		{
			base.OnLoadStarted(sender, e);
			_editButton.Enabled = false;
		}

		protected override void OnLoadFinished(object sender, EventArgs e)
		{
			base.OnLoadFinished(sender, e);

            if (CurrentWikiPage(Web.Request) != null)
            {
                _editButton.Enabled = true;
                if (NavigationItem.RightBarButtonItem == null)
                    NavigationItem.SetRightBarButtonItem(_editButton, true);
            }
            else
                NavigationItem.SetRightBarButtonItem(null, true);
		}

		protected override void Refresh()
		{
			if (ViewModel.Page != null)
				ViewModel.LoadCommand.Execute(true);
			else
				base.Refresh();
		}

        private string CurrentWikiPage(NSUrlRequest request)
        {
            var url = request.Url.AbsoluteString;
            if (!url.StartsWith("file://", StringComparison.Ordinal))
                return null;
            var s = url.LastIndexOf('/');
            if (s < 0)
                return null;
            if (url.Length < s + 1)
                return null;

            url = url.Substring(s + 1);
            return url.Substring(0, url.LastIndexOf(".html", StringComparison.Ordinal)); //Get rid of ".html"
        }

		protected override bool ShouldStartLoad(NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			try 
			{
				if (navigationType == UIWebViewNavigationType.LinkClicked) 
				{
					if (request.Url.ToString().StartsWith("wiki://", StringComparison.Ordinal))
					{
						//Load(request.Url.ToString().Substring(7));
						return false;
					}
				}
			}
			catch
			{
			}

			return true;
		}
    }
}

