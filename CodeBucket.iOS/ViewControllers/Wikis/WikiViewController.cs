using CodeBucket.Core.ViewModels.Wiki;
using UIKit;
using System.Threading.Tasks;
using WebKit;

namespace CodeBucket.ViewControllers.Wikis
{
	public class WikiViewController : WebViewController<WikiViewModel>
    {
        public WikiViewController()
        {
            NavigationItem.RightBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, (s, e) => {
                var menu = CreateExtraMenu();
                if (menu != null)
                    menu.ShowFrom(NavigationItem.RightBarButtonItem, true);
            });
        }

        private async Task HandleEditButton()
        {
//            try
//            {
//                var page = ViewModel.CurrentWikiPage(Web.Url.AbsoluteString);
//                var wiki = await Task.Run(() => ViewModel.GetApplication().Client.Users[ViewModel.Username].Repositories[ViewModel.Repository].Wikis[page].GetInfo());
//                var composer = new Composer { Title = "Edit" + Title, Text = wiki.Data };
//                composer.NewComment(this, async (text) => {
//                    try
//                    {
//                        await composer.DoWorkAsync("Saving...", () => Task.Run(() => ViewModel.GetApplication().Client.Users[ViewModel.Username].Repositories[ViewModel.Repository].Wikis[page].Update(text, Uri.UnescapeDataString("/" + page))));
//                        composer.CloseComposer();
//                        Refresh();
//                    }
//                    catch (Exception ex)
//                    {
//                        AlertDialogService.ShowAlert("Unable to update page!", ex.Message);
//                        composer.EnableSendButton = true;
//                    };
//                });
//            }
//            catch (Exception e)
//            {
//                AlertDialogService.ShowAlert("Error", e.Message);
//            }
        }

        public async override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//Stupid but I can't put this in the ViewDidLoad...
//			if (!_loaded)
//			{
//				_loaded = true;
//                var data = await ViewModel.GetData(ViewModel.Page);
//                Web.LoadRequest(new NSUrlRequest(new NSUrl(data)));
//			}
		}


        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            try 
            {
                if (navigationAction.NavigationType == WKNavigationType.LinkActivated) 
                {
                    if (navigationAction.Request.Url.ToString().Substring(0, 7).Equals("wiki://"))
                    {
                        GoToPage(navigationAction.Request.Url.ToString().Substring(7));
                        return false;
                    }
                }
            }
            catch
            {
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }

//        protected async override void Refresh()
//		{
////            var page = ViewModel.CurrentWikiPage(Web.Url.AbsoluteString);
////            if (page != null)
////            {
////                try
////                {
////                    await ViewModel.GetData(page);
////                }
////                catch (Exception e)
////                {
////                    AlertDialogService.ShowAlert("Error", e.Message);
////                }
////            }
////     
//			base.Refresh();
//		}

        private UIActionSheet CreateExtraMenu()
        {
//            var page = ViewModel.CurrentWikiPage(Web.Url.AbsoluteString);
            var sheet = new UIActionSheet();
//            var editButton = page != null ? sheet.AddButton("Edit") : -1;
//            var gotoButton = sheet.AddButton("Goto Wiki Page");
//            var showButton = page != null ? sheet.AddButton("Show in Bitbucket") : -1;
//            var cancelButton = sheet.AddButton("Cancel");
//            sheet.CancelButtonIndex = cancelButton;
//            sheet.DismissWithClickedButtonIndex(cancelButton, true);
//            sheet.Dismissed += (s, e) => 
//            {
//                BeginInvokeOnMainThread(() =>
//                {
//                if (e.ButtonIndex == editButton)
//                    HandleEditButton();
//                else if (e.ButtonIndex == gotoButton)
//                    PromptForWikiPage();
//                else if (e.ButtonIndex == showButton)
//                    ViewModel.GoToWebCommand.Execute(page);
//                });
//
//                sheet.Dispose();
//            };
//
            return sheet;
        }

        private async Task GoToPage(string page)
        {
//            try
//            {
//                var data = await ViewModel.GetData(page);
//                Web.LoadRequest(new NSUrlRequest(new NSUrl(data)));
//            }
//            catch (Exception e)
//            {
//                AlertDialogService.ShowAlert("Error", e.Message);
//            }
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

