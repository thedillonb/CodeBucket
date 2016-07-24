using System;
using UIKit;
using System.Collections.Generic;
using CodeBucket.Core.ViewModels.Source;
using System.Linq;
using WebKit;
using System.Reactive.Linq;
using CodeBucket.Client.V1;
using ReactiveUI;
using CodeBucket.Views;
using Newtonsoft.Json;
using CodeBucket.ViewControllers.Comments;

namespace CodeBucket.ViewControllers.Source
{
    public class ChangesetDiffViewController : WebViewController<ChangesetDiffViewModel>
    {
        private readonly SubtitleTitleView _titleView = new SubtitleTitleView();

        public ChangesetDiffViewController()
        {
            NavigationItem.TitleView = _titleView;
        }

        public ChangesetDiffViewController(string username, string repository, string branch, ChangesetFile model)
            : this()
        {
            ViewModel = new ChangesetDiffViewModel(username, repository, branch, model);
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            this.WhenAnyValue(x => x.ViewModel.Title, x => x.ViewModel.ChangeType)
                .Subscribe(x => _titleView.SetTitles(x.Item1, x.Item2));

            this.WhenAnyValue(x => x.ViewModel.BinaryFilePath)
                .IsNotNull()
                .Subscribe(x => LoadFile(x));

            this.WhenAnyValue(x => x.ViewModel.Patch)
                .Where(x => x != null)
                .Subscribe(x =>
            {
                var hunks = x.Select(y => new Hunk(y.OldStart, y.NewStart, y.Lines));
                var view = new DiffView { Model = new DiffViewModel(hunks) }.GenerateString();
                LoadContent(view);
            });


            //            ViewModel.Bind(x => x.IsLoading).Subscribe(x =>
            //			{
            //					if (!x && (ViewModel.File1 != null || ViewModel.File2 != null))
            //					{
            //						var sb = new StringBuilder(2000);
            //						sb.Append("a=\"");
            //						if (ViewModel.File1 != null)
            //							sb.Append(JavaScriptStringEncode(System.IO.File.ReadAllText(ViewModel.File1)));
            //						sb.Append("\";");
            //						sb.Append("b=\"");
            //						if (ViewModel.File2 != null)
            //							sb.Append(JavaScriptStringEncode(System.IO.File.ReadAllText(ViewModel.File2)));
            //						sb.Append("\";");
            //						sb.Append("diff(b,a);");
            //						ExecuteJavascript(sb.ToString());
            //					}
            //					else if (ViewModel.FilePath != null)
            //					{
            //						Web.LoadRequest(new NSUrlRequest(new NSUrl(new Uri("file://" + ViewModel.FilePath).AbsoluteUri)));
            //					}
            //			});

            //ViewModel
            //    .Comments
            //    .ChangedObservable()
            //     .Subscribe(comments =>
            //     {
            //         var slimComments = comments.Where(x => string.Equals(x.Filename, ViewModel.Filename)).Select(x => new
            //         {
            //             Id = x.CommentId,
            //             User = x.Username,
            //             Avatar = x.UserAvatarUrl,
            //             LineTo = x.LineTo,
            //             LineFrom = x.LineFrom,
            //             Content = x.ContentRendered,
            //             Date = x.UtcLastUpdated
            //         }).ToList();

            //         var c = JsonConvert.SerializeObject(slimComments);
            //         ExecuteJavascript("var a = " + c + "; setComments(a);");
            //     });
	    }

        public static int? ToNullableInt(string s)
        {
            int i;
            if (int.TryParse(s, out i)) return i;
            return null;
        }

        private class JavascriptCommentModel
        {
			public int? LineFrom { get; set; }
			public int? LineTo { get; set; }
        }

        public override bool ShouldStartLoad(WKWebView webView, WKNavigationAction navigationAction)
        {
            var url = navigationAction.Request.Url;
			if(url != null && url.Scheme.Equals("app")) {
                var func = url.Host;
		
				if(func.Equals("comment")) 
				{
                    var q = System.Web.HttpUtility.ParseQueryString(url.Query);
                    var commentModel = new JavascriptCommentModel
                    {
                        LineFrom = ToNullableInt(q["lineFrom"]),
                        LineTo = ToNullableInt(q["lineTo"])
                    };

					PromptForComment(commentModel);
                }

				return false;
            }

            return base.ShouldStartLoad(webView, navigationAction);
        }

        private void PromptForComment(JavascriptCommentModel model)
        {
			string title = "Line " + (model.LineFrom ?? model.LineTo);

            var sheet = new UIActionSheet(title);
            var addButton = sheet.AddButton("Add Comment");
            var cancelButton = sheet.AddButton("Cancel");
            sheet.CancelButtonIndex = cancelButton;
            sheet.DismissWithClickedButtonIndex(cancelButton, true);
            sheet.Dismissed += (sender, e) => {
                BeginInvokeOnMainThread(() =>
                {
                    if (e.ButtonIndex == addButton)
    					ShowCommentComposer(model.LineFrom, model.LineTo);
                });

                sheet.Dispose();
            };

            sheet.ShowInView(this.View);
        }

		private void ShowCommentComposer(int? lineFrom, int? lineTo)
        {
            NewCommentViewController.Present(this, x => ViewModel.PostComment(x, lineFrom, lineTo));
        }
    }
}

