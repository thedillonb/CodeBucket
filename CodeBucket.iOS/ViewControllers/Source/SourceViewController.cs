using System;
using UIKit;
using System.Reactive.Linq;
using CodeBucket.Core.ViewModels.Source;
using ReactiveUI;
using CodeBucket.Views;

namespace CodeBucket.ViewControllers.Source
{
	public class SourceViewController : FileSourceViewController<SourceViewModel>
    {
        private readonly UIBarButtonItem _actionButton = new UIBarButtonItem(UIBarButtonSystemItem.Action);

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

            ViewModel.WhenAnyValue(x => x.FilePath)
                .IsNotNull()
                .Subscribe(Load);

            var sourceViewModel = ViewModel;
            if (sourceViewModel != null)
                _actionButton.GetClickedObservable().InvokeCommand(sourceViewModel.ShowMenuCommand);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationItem.RightBarButtonItem = _actionButton;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            NavigationItem.RightBarButtonItem = null;
        }

        private void Load(string path)
        {
            if (ViewModel.IsText)
            {
                var content = System.IO.File.ReadAllText(path, System.Text.Encoding.UTF8);
                var fontSize = (int)UIFont.PreferredSubheadline.PointSize;
                var zoom = UIDevice.CurrentDevice.UserInterfaceIdiom != UIUserInterfaceIdiom.Phone;
                var model = new SourceBrowserModel(content, "idea", fontSize, zoom, path);
                var v = new SyntaxHighlighterView { Model = model };
                LoadContent(v.GenerateString());
            }
            else
            {
                LoadFile(path);
            }
        }
    }
}

