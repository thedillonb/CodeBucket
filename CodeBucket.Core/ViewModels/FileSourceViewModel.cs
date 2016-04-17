using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace CodeBucket.Core.ViewModels
{
    public abstract class FileSourceViewModel : BaseViewModel
    {
		private string _filePath;
		public string FilePath
		{
			get { return _filePath; }
            protected set { this.RaiseAndSetIfChanged(ref _filePath, value); }
		}

        private bool _isText;
		public bool IsText
		{
            get { return _isText; }
            protected set { this.RaiseAndSetIfChanged(ref _isText, value); }
		}

		public string Title { get; protected set; }

		public string HtmlUrl { get; protected set; }

		public ICommand GoToHtmlUrlCommand
		{
			get { return new MvxCommand(() => ShowViewModel<WebBrowserViewModel>(new WebBrowserViewModel.NavObject { Url = HtmlUrl }), () => !string.IsNullOrEmpty(HtmlUrl)); }
		}
    }
}

