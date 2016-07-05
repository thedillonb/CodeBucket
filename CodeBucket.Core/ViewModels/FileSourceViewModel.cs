using System;
using System.Reactive.Linq;
using ReactiveUI;

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

        private string _htmlUrl;
        public string HtmlUrl
        {
            get { return _htmlUrl; }
            protected set { this.RaiseAndSetIfChanged(ref _htmlUrl, value); }
        }

        public IReactiveCommand<object> GoToHtmlUrlCommand { get; }

        protected FileSourceViewModel()
        {
            GoToHtmlUrlCommand = ReactiveCommand.Create(
                this.WhenAnyValue(x => x.HtmlUrl)
                .Select(x => !string.IsNullOrEmpty(x)));

            GoToHtmlUrlCommand
                .Select(_ => new WebBrowserViewModel(HtmlUrl))
                .Subscribe(NavigateTo);
        }
    }
}

