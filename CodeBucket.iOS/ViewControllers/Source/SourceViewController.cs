using System;
using UIKit;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Source;
using ReactiveUI;
using CodeBucket.Views;
using Splat;
using CodeBucket.Core.Services;

namespace CodeBucket.ViewControllers.Source
{
	public class SourceViewController : WebViewController<SourceViewModel>
    {
        private readonly UIBarButtonItem _actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);
        private string _loadedPath;

        public SourceViewController()
        {
            NavigationItem.RightBarButtonItem = _actionButton;

            OnActivation(disposable =>
            {
                _actionButton
                    .GetClickedObservable()
                    .Select(x => (object)x)
                    .BindCommand(ViewModel.ShowMenuCommand)
                    .AddTo(disposable);

                this.WhenAnyValue(x => x.ViewModel.FilePath)
                    .Where(x => x != null)
                    .Subscribe(Load)
                    .AddTo(disposable);
            });
        }

        public SourceViewController(string username, string repository, string branch, string path)
            : this()
        {
            ViewModel = new SourceViewModel(username, repository, branch, path);
        }


        private void Load(string path)
        {
            if (path == _loadedPath)
                return;

            _loadedPath = path;

            if (ViewModel.IsText)
            {
                if (ViewModel.IsMarkdown)
                {
                    var converter = Locator.Current.GetService<IMarkdownService>();
                    var content = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                    var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
                    var markdownContent = converter.ConvertMarkdown(content);
                    var model = new DescriptionModel(markdownContent, fontSize);
                    var v = new MarkdownView { Model = model };
                    LoadContent(v.GenerateString());
                }
                else
                {
                    var content = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                    var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
                    var zoom = UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Phone;
                    var model = new SourceBrowserModel(content, "idea", fontSize, zoom, path);
                    var v = new SyntaxHighlighterView { Model = model };
                    LoadContent(v.GenerateString());
                }
            }
            else
            {
                LoadFile(path);
            }
        }
    }
}

