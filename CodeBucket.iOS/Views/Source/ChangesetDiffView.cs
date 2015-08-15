using System;
using CodeBucket.iOS.Views.Source;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Collections.Generic;
using CodeBucket.Core.ViewModels.Source;
using CodeFramework.iOS.ViewControllers;
using CodeFramework.iOS.Utils;
using CodeFramework.Core.Services;
using System.Text;
using System.Linq;

namespace CodeBucket.ViewControllers
{
	public class ChangesetDiffView : FileSourceView
    {
		private readonly IJsonSerializationService _serializationService = Cirrious.CrossCore.Mvx.Resolve<IJsonSerializationService>();
		private bool _domLoaded = false;
		private List<string> _toBeExecuted = new List<string>();

		public new ChangesetDiffViewModel ViewModel
		{
			get { return (ChangesetDiffViewModel)base.ViewModel; }
			set { base.ViewModel = value; }
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ViewModel.Bind(x => x.IsLoading, x =>
			{
					if (!x && (ViewModel.File1 != null || ViewModel.File2 != null))
					{
						var sb = new StringBuilder(2000);
						sb.Append("a=\"");
						if (ViewModel.File1 != null)
							sb.Append(JavaScriptStringEncode(System.IO.File.ReadAllText(ViewModel.File1)));
						sb.Append("\";");
						sb.Append("b=\"");
						if (ViewModel.File2 != null)
							sb.Append(JavaScriptStringEncode(System.IO.File.ReadAllText(ViewModel.File2)));
						sb.Append("\";");
						sb.Append("diff(b,a);");
						ExecuteJavascript(sb.ToString());
					}
					else if (ViewModel.FilePath != null)
					{
						Web.LoadRequest(new NSUrlRequest(new NSUrl(new Uri("file://" + ViewModel.FilePath).AbsoluteUri)));
					}
			});

			ViewModel.BindCollection(x => x.Comments, e =>
			{
				//Convert it to something light weight
				var slimComments = ViewModel.Comments.Items.Where(x => string.Equals(x.Filename, ViewModel.Filename)).Select(x => new { 
					Id = x.CommentId, User = x.Username, Avatar = x.UserAvatarUrl, LineTo = x.LineTo, LineFrom = x.LineFrom,
					Content = x.ContentRendered, Date = x.UtcLastUpdated
				}).ToList();

				var c = _serializationService.Serialize(slimComments);
				ExecuteJavascript("var a = " + c + "; setComments(a);");
			});
		}

		private bool _isLoaded;
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			if (!_isLoaded)
			{
				var path = System.IO.Path.Combine(NSBundle.MainBundle.BundlePath, "Diff", "diffindex.html");
				var uri = Uri.EscapeUriString("file://" + path) + "#" + Environment.TickCount;
				Web.LoadRequest(new MonoTouch.Foundation.NSUrlRequest(new MonoTouch.Foundation.NSUrl(uri)));
				_isLoaded = true;
			}
		}

        private class JavascriptCommentModel
        {
			public int? LineFrom { get; set; }
			public int? LineTo { get; set; }
        }

        protected override bool ShouldStartLoad(NSUrlRequest request, UIWebViewNavigationType navigationType)
        {
            var url = request.Url;
			if(url != null && url.Scheme.Equals("app")) {
                var func = url.Host;

				if (func.Equals("ready"))
				{
					_domLoaded = true;
					foreach (var e in _toBeExecuted)
						Web.EvaluateJavascript(e);
				}
				else if(func.Equals("comment")) 
				{
					var commentModel = _serializationService.Deserialize<JavascriptCommentModel>(UrlDecode(url.Fragment));
					PromptForComment(commentModel);
                }

				return false;
            }

            return base.ShouldStartLoad(request, navigationType);
        }

		private void ExecuteJavascript(string data)
		{
			if (_domLoaded)
				InvokeOnMainThread(() => Web.EvaluateJavascript(data));
			else
				_toBeExecuted.Add(data);
		}

        private void PromptForComment(JavascriptCommentModel model)
        {
			string title = "Line ".t() + (model.LineFrom ?? model.LineTo);

            var sheet = MonoTouch.Utilities.GetSheet(title);
            var addButton = sheet.AddButton("Add Comment".t());
            var cancelButton = sheet.AddButton("Cancel".t());
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (sender, e) => {
                BeginInvokeOnMainThread(() =>
                {
                if (e.ButtonIndex == addButton)
					ShowCommentComposer(model.LineFrom, model.LineTo);
                });
            };

            sheet.ShowInView(this.View);
        }

		private void ShowCommentComposer(int? lineFrom, int? lineTo)
        {
            var composer = new Composer();
			composer.NewComment(this, async (text) => {
				try
				{
					await composer.DoWorkAsync("Commenting...", () => ViewModel.PostComment(text, lineFrom, lineTo));
					composer.CloseComposer();
				}
				catch (Exception e)
				{
					MonoTouch.Utilities.ShowAlert("Unable to Comment".t(), e.Message);
					composer.EnableSendButton = true;
				}
            });
        }
    }
}

