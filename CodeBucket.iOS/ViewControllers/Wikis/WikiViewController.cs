using CodeBucket.Core.ViewModels.Wiki;
using UIKit;
using System.Threading.Tasks;
using WebKit;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Wikis
{
	public class WikiViewController : WebViewController<WikiViewModel>
    {
        private string _content;
        public string Content
        {
            get { return _content; }
            private set { this.RaiseAndSetIfChanged(ref _content, value); }
        }

        public WikiViewController()
        {
            var actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
            NavigationItem.RightBarButtonItem = actionButton;

            OnActivation(disposable =>
            {
                actionButton
                    .GetClickedObservable()
                    .SelectUnit()
                    .BindCommand(this, x => x.ViewModel.ShowMenuCommand)
                    .AddTo(disposable);
            });
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.WhenAnyValue(x => x.Content)
                .Select(x => new DescriptionModel(x, (int)UIFont.PreferredSubheadline.PointSize, true))
                .Select(x => new MarkdownView { Model = x }.GenerateString())
                .Subscribe(LoadContent);

            OnActivation(disposable =>
            {
                this.WhenAnyValue(x => x.ViewModel.Content)
                    .Subscribe(x => Content = x)
                    .AddTo(disposable);
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

        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            try 
            {
                if (navigationAction.NavigationType == WKNavigationType.LinkActivated) 
                {
                    ViewModel.GoToWebCommand.ExecuteNow(navigationAction.Request.Url.ToString());
                    return false;
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
                    //GoToPage(alert.GetTextField(0).Text);
                    //ViewModel.GoToPageCommand.Execute(alert.GetTextField(0).Text);
                }
            };
            alert.Show();
        }
    }
}

