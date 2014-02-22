using System;
using CodeFramework.iOS.Views;
using CodeBucket.Core.ViewModels.Wiki;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Utils;
using System.Threading.Tasks;

namespace CodeBucket.iOS
{
	public class WikiView : WebView
    {
		private bool _loaded;

		public new WikiViewModel ViewModel
		{
			get { return (WikiViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

        public WikiView()
            : base(true, true)
        {
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => {
                var menu = CreateExtraMenu();
                if (menu != null)
                    menu.ShowFrom(NavigationItem.RightBarButtonItem, true);
            });
        }

        private async Task HandleEditButton()
        {
            try
            {
                var page = ViewModel.CurrentWikiPage(Web.Request.Url.AbsoluteString);
                var wiki = await Task.Run(() => ViewModel.GetApplication().Client.Users[ViewModel.Username].Repositories[ViewModel.Repository].Wikis[page].GetInfo());
                var composer = new Composer { Title = "Edit".t() + Title, Text = wiki.Data, ActionButtonText = "Save".t() };
                composer.NewComment(this, async (text) => {
                    try
                    {
                        await composer.DoWorkAsync("Saving...", () => Task.Run(() => ViewModel.GetApplication().Client.Users[ViewModel.Username].Repositories[ViewModel.Repository].Wikis[page].Update(text, Uri.UnescapeDataString("/" + page))));
                        composer.CloseComposer();
                        Refresh();
                    }
                    catch (Exception ex)
                    {
                        MonoTouch.Utilities.ShowAlert("Unable to update page!", ex.Message);
                        composer.EnableSendButton = true;
                    };
                });
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.ShowAlert("Error", e.Message);
            }
        }

        public async override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Stupid but I can't put this in the ViewDidLoad...
			if (!_loaded)
			{
				_loaded = true;
                var data = await ViewModel.GetData(ViewModel.Page);
                Web.LoadRequest(new NSUrlRequest(new NSUrl(data)));
			}
		}


        protected override bool ShouldStartLoad(NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            try 
            {
                if (navigationType == UIWebViewNavigationType.LinkClicked) 
                {
                    if (request.Url.ToString().Substring(0, 7).Equals("wiki://"))
                    {
                        GoToPage(request.Url.ToString().Substring(7));
                        return false;
                    }
                }
            }
            catch
            {
            }

            return base.ShouldStartLoad(request, navigationType);
        }

        protected async override void Refresh()
		{
            var page = ViewModel.CurrentWikiPage(Web.Request.Url.AbsoluteString);
            if (page != null)
            {
                try
                {
                    await ViewModel.GetData(page);
                }
                catch (Exception e)
                {
                    MonoTouch.Utilities.ShowAlert("Error", e.Message);
                }
            }
     
			base.Refresh();
		}

        private UIActionSheet CreateExtraMenu()
        {
            var repoModel = ViewModel.Repository;
            if (repoModel == null)
                return null;

            var page = ViewModel.CurrentWikiPage(Web.Request.Url.AbsoluteString);
            var sheet = MonoTouch.Utilities.GetSheet("Wiki");
            var editButton = page != null ? sheet.AddButton("Edit".t()) : -1;
            var gotoButton = sheet.AddButton("Goto Wiki Page".t());
            var showButton = page != null ? sheet.AddButton("Show in Bitbucket") : -1;
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Clicked += (s, e) => 
            {
                if (e.ButtonIndex == editButton)
                    HandleEditButton();
                else if (e.ButtonIndex == gotoButton)
                    PromptForWikiPage();
                else if (e.ButtonIndex == showButton)
                    ViewModel.GoToWebCommand.Execute(page);
            };

            return sheet;
        }

        private async Task GoToPage(string page)
        {
            try
            {
                var data = await ViewModel.GetData(page);
                Web.LoadRequest(new NSUrlRequest(new NSUrl(data)));
            }
            catch (Exception e)
            {
                MonoTouch.Utilities.ShowAlert("Error", e.Message);
            }
        }

        private void PromptForWikiPage()
        {
            var alert = new UIAlertView
            {
                Title = "Goto Wiki Page",
                Message = "What is the title of the Wiki page you'd like to goto?",
                AlertViewStyle = UIAlertViewStyle.PlainTextInput
            };

            alert.CancelButtonIndex = alert.AddButton("Cancel");
            alert.DismissWithClickedButtonIndex(alert.CancelButtonIndex, true);
            var gotoButton = alert.AddButton("Go");
            alert.Dismissed += (sender, e) =>
            {
                if (e.ButtonIndex == gotoButton)
                {
                    GoToPage(alert.GetTextField(0).Text);
                    //ViewModel.GoToPageCommand.Execute(alert.GetTextField(0).Text);
                }
            };
            alert.Show();
        }
    }
}

